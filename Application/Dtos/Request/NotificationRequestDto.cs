namespace Application.Dtos.Request
{
    public class NotificationRequestDto
    {
        public long ChannelId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
