namespace Domain.Entities
{
    public class GymAccessType
    {
        public int GymAccessTypeId { get; set; }

        public int IdGym { get; set; }

        public int IdAccessType { get; set; }

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual AccessType IdAccessTypeNavigation { get; set; } = null!;
    }
}
