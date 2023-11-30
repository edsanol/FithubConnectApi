namespace Application.Dtos.Request
{
    public class AthleteMembershipDto
    {
        public int AthleteId { get; set; }
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
