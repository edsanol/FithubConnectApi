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
            _smtpClient = new SmtpClient(configuration["Email:Smtp:Host"])
            {
                Port = int.Parse(configuration["Email:Smtp:Port"]!),
                Credentials = new NetworkCredential(configuration["Email:Smtp:Username"], configuration["Email:Smtp:Password"]),
                EnableSsl = bool.Parse(configuration["Email:Smtp:EnableSsl"]!)
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
