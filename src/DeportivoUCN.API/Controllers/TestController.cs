using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(IEmailService emailService) : ControllerBase
{
    [HttpGet]
    public IActionResult TestConnection()
    {
        return Ok(new { mensaje = "¡Conexión exitosa con el backend de Deportivo UCN!" });
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string email = "gabriel.briones@alumnos.ucn.cl")
    {
        var subject = "Prueba de Integración Resend - Deportivo UCN";
        var body = "Hola Gabriel,\n\nEste es un correo de prueba enviado desde el sistema de Deportivo UCN utilizando la integración con Resend.\n\nSi estás recibiendo este mensaje, la integración y la plantilla HTML de correo están funcionando correctamente.";
        
        await emailService.SendEmailAsync(email, subject, body);
        return Ok(new { mensaje = $"Correo de prueba enviado a {email}" });
    }

    [HttpPost("email/arriendo")]
    public async Task<IActionResult> TestArriendoEmail([FromQuery] string email = "gabriel.briones@alumnos.ucn.cl")
    {
        var subject = "Confirmación de Solicitud de Arriendo - Deportivo UCN";
        var body = $"Hola Gabriel Briones,\n\n" +
                   $"Hemos recibido tu solicitud de arriendo de cancha en Deportivo UCN con los siguientes detalles:\n\n" +
                   $"• Cancha: Cancha 1 - Pasto Sintético\n" +
                   $"• Fecha: {DateTime.Now.AddDays(1):dd/MM/yyyy}\n" +
                   $"• Horario: 16:00 - 17:00 hrs\n" +
                   $"• Monto abonado: $12.500\n" +
                   $"• Monto restante a pagar: $12.500\n" +
                   $"• Estado: Pendiente de aprobación por administración\n\n" +
                   $"Un administrador revisará tu solicitud a la brevedad. Recibirás una notificación por correo una vez que sea procesada.";

        await emailService.SendEmailAsync(email, subject, body);
        return Ok(new { mensaje = $"Correo de arriendo enviado a {email}" });
    }

    [HttpPost("email/aceptada")]
    public async Task<IActionResult> TestAceptadaEmail([FromQuery] string email = "gabriel.briones@alumnos.ucn.cl")
    {
        var subject = "¡Solicitud Aprobada! Confirmación de Arriendo - Deportivo UCN";
        var body = $"Hola Gabriel Briones,\n\n" +
                   $"¡Excelentes noticias! Tu solicitud de arriendo de cancha ha sido ACEPTADA por la administración.\n\n" +
                   $"Detalles del Arriendo:\n" +
                   $"• Cancha: Cancha 1 - Pasto Sintético\n" +
                   $"• Fecha: {DateTime.Now.AddDays(1):dd/MM/yyyy}\n" +
                   $"• Horario: 16:00 - 17:00 hrs\n" +
                   $"• Estado: ACEPTADA / CONFIRMADA\n" +
                   $"• Observaciones del administrador: Pago del abono verificado correctamente.\n\n" +
                   $"Te esperamos en el recinto deportivo a la hora agendada. ¡Gracias por usar Deportivo UCN!";

        await emailService.SendEmailAsync(email, subject, body);
        return Ok(new { mensaje = $"Correo de aceptación enviado a {email}" });
    }

    [HttpPost("email/rechazada")]
    public async Task<IActionResult> TestRechazadaEmail([FromQuery] string email = "gabriel.briones@alumnos.ucn.cl")
    {
        var subject = "Solicitud Rechazada / Cancelada - Deportivo UCN";
        var body = $"Hola Gabriel Briones,\n\n" +
                   $"Te informamos que tu solicitud de arriendo para la cancha 'Cancha 1 - Pasto Sintético' el día {DateTime.Now.AddDays(1):dd/MM/yyyy} de 16:00 a 17:00 hrs ha sido RECHAZADA / CANCELADA por la administración.\n\n" +
                   $"Motivo / Observaciones del administrador:\n" +
                   $"La cancha se encontrará en mantenimiento por trabajos de iluminación durante la fecha seleccionada.\n\n" +
                   $"Si tienes preguntas o deseas realizar otra reserva, comunícate con la administración de Deportivo UCN.";

        await emailService.SendEmailAsync(email, subject, body);
        return Ok(new { mensaje = $"Correo de rechazo enviado a {email}" });
    }
}


