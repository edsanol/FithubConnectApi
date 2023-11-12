namespace Application.Dtos.Response
{
    public class MembershipSelectResponseDto
    {
        public int MembershipID { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public int GymID { get; set; }
    }
}
