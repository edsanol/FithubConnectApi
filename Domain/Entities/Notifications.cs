namespace Domain.Entities
{
    public class Notifications
    {
        public long NotificationId { get; set; }

        public long IdChannel { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SendAt { get; set; }

        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public virtual Channels IdChannelNavigation { get; set; } = null!;
    }
}
