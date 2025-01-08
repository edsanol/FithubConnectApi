namespace Domain.Entities
{
    public class ChannelUsers
    {
        public long ChannelUsersId { get; set; }

        public long IdChannel { get; set; }

        public int IdAthlete { get; set; }

        public virtual Athlete IdAthleteNavigation { get; set; } = null!;

        public virtual Channels IdChannelNavigation { get; set; } = null!;
    }
}
