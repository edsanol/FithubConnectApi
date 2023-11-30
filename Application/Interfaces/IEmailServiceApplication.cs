namespace Application.Interfaces
{
    public interface IEmailServiceApplication
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
