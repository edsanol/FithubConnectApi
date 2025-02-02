﻿namespace Application.Dtos.Response
{
    public class NotificationResponseDto
    {
        public long NotificationId { get; set; }
        public long ChannelId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime SendAt { get; set; }
    }
}
