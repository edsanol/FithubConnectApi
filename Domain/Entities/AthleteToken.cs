namespace Domain.Entities
{
    public partial class AthleteToken
    {
        public int TokenID { get; set; }
        public int IdAthlete { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateOnly Expires { get; set; }
        public bool Revoked { get; set; }
        public virtual Athlete IdAthleteNavigation { get; set; } = null!;
    }
}
