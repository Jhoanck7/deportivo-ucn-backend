using DeportivoUCN.Application.Interfaces;

namespace DeportivoUCN.Application.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        Console.WriteLine($"[EMAIL MOCK] Sending email to {toEmail}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");
        return Task.CompletedTask;
    }
}
