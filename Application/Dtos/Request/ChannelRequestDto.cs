namespace Application.Dtos.Request
{
    public class ChannelRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
