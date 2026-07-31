using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DeportivoUCN.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeportivoUCN.Application.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var apiKey = _configuration["Resend:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "re_YOUR_RESEND_API_KEY")
        {
            apiKey = _configuration["RESEND_API_KEY"];
        }

        var fromEmail = _configuration["Resend:FromEmail"];
        if (string.IsNullOrWhiteSpace(fromEmail) || fromEmail == "Deportivo UCN <onboarding@resend.dev>")
        {
            fromEmail = _configuration["RESEND_FROM_EMAIL"] ?? "Deportivo UCN <onboarding@resend.dev>";
        }

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "re_YOUR_RESEND_API_KEY")
        {
            _logger.LogWarning("[EMAIL MOCK] Resend API Key not configured. Simulating email send to {ToEmail}", toEmail);
            Console.WriteLine($"[EMAIL MOCK] Sending email to {toEmail}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            return;
        }

        try
        {
            var requestUri = "https://api.resend.com/emails";

            var payload = new
            {
                from = fromEmail,
                to = new[] { toEmail },
                subject = subject,
                html = BuildHtmlTemplate(subject, body)
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Email sent successfully via Resend to {ToEmail}. Response: {Response}", toEmail, responseContent);
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send email via Resend to {ToEmail}. Status: {StatusCode}, Error: {Error}", 
                    toEmail, response.StatusCode, errorResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending email via Resend to {ToEmail}", toEmail);
        }
    }

    private static string BuildHtmlTemplate(string title, string body)
    {
        var htmlBody = body.Replace("\n", "<br>");

        string badgeBg = "rgba(255, 255, 255, 0.15)";
        string badgeText = "Notificación Oficial";

        if (title.Contains("Aprobada", StringComparison.OrdinalIgnoreCase) || title.Contains("Confirmación", StringComparison.OrdinalIgnoreCase))
        {
            badgeBg = "#10b981";
            badgeText = "SOLICITUD APROBADA";
        }
        else if (title.Contains("Rechazada", StringComparison.OrdinalIgnoreCase) || title.Contains("Cancelada", StringComparison.OrdinalIgnoreCase))
        {
            badgeBg = "#ef4444";
            badgeText = "SOLICITUD CANCELADA";
        }
        else if (title.Contains("Solicitud", StringComparison.OrdinalIgnoreCase))
        {
            badgeBg = "#f59e0b";
            badgeText = "SOLICITUD REGISTRADA";
        }

        return $$"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{{title}}</title>
            <style>
                @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');
                
                body {
                    font-family: 'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                    background-color: #f8fafc;
                    margin: 0;
                    padding: 30px 12px;
                    color: #0f172a;
                    -webkit-font-smoothing: antialiased;
                }
                .wrapper {
                    max-width: 600px;
                    margin: 0 auto;
                }
                .card {
                    background: #ffffff;
                    border-radius: 20px;
                    overflow: hidden;
                    box-shadow: 0 10px 30px -5px rgba(7, 14, 39, 0.08), 0 4px 6px -2px rgba(7, 14, 39, 0.04);
                    border: 1px solid #e2e8f0;
                }
                .header-banner {
                    background-color: #070e27;
                    background-image: linear-gradient(135deg, #070e27 0%, #1e3a8a 100%);
                    padding: 36px 28px;
                    text-align: center;
                    position: relative;
                    border-bottom: 4px solid #e2583e;
                }
                .brand-title {
                    color: #ffffff;
                    font-size: 26px;
                    font-weight: 800;
                    letter-spacing: -0.5px;
                    margin: 0;
                    text-transform: uppercase;
                }
                .brand-subtitle {
                    color: #94a3b8;
                    font-size: 13px;
                    font-weight: 600;
                    letter-spacing: 1px;
                    margin-top: 4px;
                    text-transform: uppercase;
                }
                .status-badge {
                    display: inline-block;
                    background-color: {{badgeBg}};
                    color: #ffffff;
                    padding: 6px 16px;
                    border-radius: 50px;
                    font-size: 11px;
                    font-weight: 800;
                    letter-spacing: 1.2px;
                    margin-top: 16px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.15);
                }
                .content {
                    padding: 36px 32px;
                }
                .email-subject-title {
                    color: #070e27;
                    font-size: 20px;
                    font-weight: 800;
                    margin-top: 0;
                    margin-bottom: 20px;
                    line-height: 1.3;
                }
                .body-box {
                    color: #334155;
                    font-size: 15px;
                    line-height: 1.7;
                }
                .body-box ul {
                    padding-left: 20px;
                    margin: 16px 0;
                }
                .body-box li {
                    margin-bottom: 8px;
                }
                .btn-container {
                    text-align: center;
                    margin-top: 32px;
                    margin-bottom: 12px;
                }
                .btn-primary {
                    display: inline-block;
                    background: #e2583e;
                    color: #ffffff !important;
                    font-weight: 700;
                    font-size: 14px;
                    padding: 14px 28px;
                    border-radius: 12px;
                    text-decoration: none;
                    box-shadow: 0 4px 12px rgba(226, 88, 62, 0.3);
                }
                .footer {
                    background: #f8fafc;
                    padding: 24px;
                    text-align: center;
                    font-size: 12px;
                    color: #64748b;
                    border-top: 1px solid #e2e8f0;
                    line-height: 1.5;
                }
                .footer strong {
                    color: #070e27;
                }
            </style>
        </head>
        <body>
            <div class="wrapper">
                <div class="card">
                    <div class="header-banner">
                        <h1 class="brand-title">Deportivo UCN</h1>
                        <div class="brand-subtitle">Plataforma de Gestión Deportiva</div>
                        <div class="status-badge">{{badgeText}}</div>
                    </div>
                    <div class="content">
                        <h2 class="email-subject-title">{{title}}</h2>
                        <div class="body-box">
                            {{htmlBody}}
                        </div>
                        <div class="btn-container">
                            <a href="http://localhost:4200" class="btn-primary">Ir a Deportivo UCN</a>
                        </div>
                    </div>
                    <div class="footer">
                        <p>Este correo fue generado de forma automática por el sistema <strong>Deportivo UCN</strong>.</p>
                        <p>© {{DateTime.Now.Year}} Universidad Católica del Norte — Antofagasta, Chile.</p>
                    </div>
                </div>
            </div>
        </body>
        </html>
        """;
    }
}

