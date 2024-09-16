using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application.Services
{
    public class EmailServiceApplication : IEmailServiceApplication
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        public EmailServiceApplication(IConfiguration configuration)
        {
            // Configuración de email SMTP
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? configuration["Email:Smtp:Host"];
            var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT") ?? configuration["Email:Smtp:Port"];
            var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? configuration["Email:Smtp:Username"];
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? configuration["Email:Smtp:Password"];
            var smtpEnableSsl = Environment.GetEnvironmentVariable("SMTP_ENABLESSL") ?? configuration["Email:Smtp:EnableSsl"];
            var fromAddress = Environment.GetEnvironmentVariable("SMTP_FROM_ADDRESS") ?? configuration["Email:FromAddress"];

            if (smtpHost == null || smtpPort == null || smtpUsername == null || smtpPassword == null || smtpEnableSsl == null || fromAddress == null)
            {
                throw new Exception("One or more required email environment variables are missing.");
            }

            int port = int.Parse(smtpPort); // Aquí asegúrate de que '587' sea tratado como un entero, no como una cadena
            bool enableSsl = bool.Parse(smtpEnableSsl);

            _smtpClient = new SmtpClient(smtpHost)
            {
                Port = port,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            _fromAddress = configuration["Email:FromAddress"]!;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromAddress, to, subject, body)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
