namespace Domain.Entities
{
    public class AccessType
    {
        public int AccessTypeId { get; set; }

        public string AccessTypeName { get; set; } = null!;

        public bool Status { get; set; }

        public virtual ICollection<GymAccessType> GymAccessTypes { get; set; } = new List<GymAccessType>();
    }
}
