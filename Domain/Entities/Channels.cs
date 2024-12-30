namespace Domain.Entities
{
    public class Channels
    {
        public long ChannelId { get; set; }

        public string ChannelName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int IdGym { get; set; }

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual ICollection<ChannelUsers> ChannelUsers { get; set; } = new List<ChannelUsers>();

        public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();
    }
}
