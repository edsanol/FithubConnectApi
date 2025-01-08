namespace Application.Dtos.Request
{
    public class ChannelRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new List<int>();
        public bool AllUsersSelected { get; set; } = false;
        public List<int> DeselectedUserIds { get; set; } = new List<int>();
        public bool AllUsersSelectedByMembersip { get; set; } = false;
        public List<int> MembershipIds { get; set; } = new List<int>();
    }
}
