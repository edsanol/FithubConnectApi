namespace Application.Dtos.Request
{
    public class NotificationRequestDto
    {
        public long ChannelId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "message";
        public string Title { get; set; } = string.Empty;
    }
}
