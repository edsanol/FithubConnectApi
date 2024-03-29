namespace Domain.Entities
{
    public partial class GymToken
    {
        public int TokenID { get; set; }
        public int IdGym { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateOnly Expires { get; set; }
        public bool Revoked { get; set; }
        public virtual Gym IdGymNavigation { get; set; } = null!;
    }
}
