namespace Application.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(
            List<string> deviceTokens,
            string title,
            string body
        );
    }
}
