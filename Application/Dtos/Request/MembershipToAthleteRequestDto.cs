namespace Application.Dtos.Request
{
    public class MembershipToAthleteRequestDto
    {
        public int AthleteId { get; set; }
        public int MembershipId { get; set; }
        public DateOnly? StartMembershipDate { get; set; }
    }
}
