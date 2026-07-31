using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace DeportivoUCN.API.Controllers;

public class InitPaymentRequest
{
    public int BookingId { get; set; }
}

public class TransbankInitResponse
{
    public string Token { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class TransbankCommitResponse
{
    public string Status { get; set; } = string.Empty;
    public string Authorization_Code { get; set; } = string.Empty;
}

[ApiController]
[Route("api/transacciones")]
public class PaymentController(DeportivoUCNContext context, IConfiguration configuration) : ControllerBase
{
    // Almacenamiento en memoria para asociar tokens de Transbank con las reservas locales
    private static readonly Dictionary<string, (int BookingId, string ReturnType)> Transactions = new();

    [HttpPost("iniciar")]
    public async Task<IActionResult> Iniciar([FromBody] InitPaymentRequest request, [FromQuery] string returnType = "SESSION_RENT")
    {
        var booking = await context.Bookings.Include(b => b.Court).FirstOrDefaultAsync(b => b.Id == request.BookingId);
        if (booking == null)
        {
            return NotFound(new { message = "Reserva no encontrada" });
        }

        using var client = new HttpClient();
        // Evitar bloqueo WAF agregando un User-Agent real
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        // Credenciales oficiales de Transbank para el Ambiente de Integración (Pruebas)
        client.DefaultRequestHeaders.Add("Tbk-Api-Key-Id", "597055555532");
        client.DefaultRequestHeaders.Add("Tbk-Api-Key-Secret", "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C");

        var returnUrl = $"{Request.Scheme}://{Request.Host}/api/transacciones/confirmar-real";
        var payload = new
        {
            buy_order = booking.Id.ToString(),
            session_id = returnType,
            amount = (int)booking.DepositAmount,
            return_url = returnUrl
        };

        try
        {
            var response = await client.PostAsJsonAsync("https://webpay3gint.transbank.cl/rswebpaytransaction/api/webpay/v1.2/transactions", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Transbank API returned status code " + response.StatusCode + ": " + error);
                return BadRequest(new { message = "Error al iniciar pago en Transbank", detail = error });
            }

            var result = await response.Content.ReadFromJsonAsync<TransbankInitResponse>();
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                return BadRequest(new { message = "Error al obtener token de Transbank" });
            }

            // Registrar transacción en memoria
            Transactions[result.Token] = (booking.Id, returnType);

            return Ok(new
            {
                message = "Transacción iniciada con Webpay de Transbank",
                data = new
                {
                    token = result.Token,
                    urlRedireccion = result.Url, // URL real de Transbank Integration
                    amount = booking.DepositAmount,
                    courtName = booking.Court?.Name
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPCIÓN al conectar con Transbank: " + ex.ToString());
            return StatusCode(500, new { message = "Error de conexión con la API de Transbank", error = ex.Message });
        }
    }

    public class ConfirmPaymentRequest
    {
        public string TokenWs { get; set; } = string.Empty;
        public int BookingId { get; set; }
    }

    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar([FromBody] ConfirmPaymentRequest request)
    {
        var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
        if (booking == null)
        {
            return NotFound(new { message = "Reserva no encontrada" });
        }

        // Actualizar estado de la reserva a Confirmada tras el pago
        booking.Status = BookingStatus.Confirmed;
        booking.AdminNotes = "Pago verificado por Webpay Simulado Local. Token: " + request.TokenWs;
        await context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(request.TokenWs))
        {
            Transactions.Remove(request.TokenWs);
        }

        return Ok(new
        {
            message = "Pago confirmado con éxito",
            data = new
            {
                bookingId = booking.Id,
                status = booking.Status.ToString()
            }
        });
    }

    [HttpGet("confirmar-real")]
    [HttpPost("confirmar-real")]
    public async Task<IActionResult> ConfirmarReal(
        [FromQuery(Name = "token_ws")] string? tokenWsQuery,
        [FromForm(Name = "token_ws")] string? tokenWsForm,
        [FromQuery(Name = "tbk_token")] string? tbkTokenQuery,
        [FromForm(Name = "tbk_token")] string? tbkTokenForm,
        [FromQuery(Name = "tbk_id_sesion")] string? tbkIdSesionQuery,
        [FromForm(Name = "tbk_id_sesion")] string? tbkIdSesionForm)
    {
        string? token = !string.IsNullOrEmpty(tokenWsQuery) ? tokenWsQuery : tokenWsForm;
        string? tbkToken = !string.IsNullOrEmpty(tbkTokenQuery) ? tbkTokenQuery : tbkTokenForm;
        bool isSuccess = !string.IsNullOrEmpty(token);

        // Si el usuario cancela en el portal de Transbank, retorna tbk_token
        if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(tbkToken))
        {
            token = tbkToken;
            isSuccess = false;
        }

        string frontendBase = configuration["FrontendUrl"] ?? "http://localhost:4200";

        if (string.IsNullOrEmpty(token) || !Transactions.TryGetValue(token, out var txData))
        {
            return Redirect($"{frontendBase}/rent?payment=cancel");
        }

        string targetUrl = txData.ReturnType == "SESSION_TEST"
            ? $"{frontendBase}/reserva-canchas-test.html"
            : $"{frontendBase}/rent";

        if (!isSuccess)
        {
            Transactions.Remove(token);
            return Redirect($"{targetUrl}?payment=cancel&bookingId={txData.BookingId}");
        }

        // Confirmar transacción (Commit) con la API de Transbank
        using var client = new HttpClient();
        // Evitar bloqueo WAF agregando un User-Agent real
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Tbk-Api-Key-Id", "597055555532");
        client.DefaultRequestHeaders.Add("Tbk-Api-Key-Secret", "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C");

        var response = await client.PutAsync($"https://webpay3gint.transbank.cl/rswebpaytransaction/api/webpay/v1.2/transactions/{token}", null);

        if (!response.IsSuccessStatusCode)
        {
            Transactions.Remove(token);
            return Redirect($"{targetUrl}?payment=cancel&bookingId={txData.BookingId}");
        }

        var commitResult = await response.Content.ReadFromJsonAsync<TransbankCommitResponse>();

        if (commitResult != null && commitResult.Status == "AUTHORIZED")
        {
            // Actualizar la reserva en base de datos
            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == txData.BookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Confirmed;
                booking.AdminNotes = $"Pago aprobado por Webpay Plus. Código Autorización: {commitResult.Authorization_Code}";
                await context.SaveChangesAsync();
            }

            Transactions.Remove(token);
            return Redirect($"{targetUrl}?payment=success&bookingId={txData.BookingId}&token_ws={token}");
        }
        else
        {
            Transactions.Remove(token);
            return Redirect($"{targetUrl}?payment=cancel&bookingId={txData.BookingId}");
        }
    }
}
