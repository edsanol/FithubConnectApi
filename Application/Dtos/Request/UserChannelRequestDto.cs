namespace Application.Dtos.Request
{
    public class UserChannelRequestDto
    {
        public int ChannelId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
