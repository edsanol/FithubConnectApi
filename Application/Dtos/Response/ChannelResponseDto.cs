namespace Application.Dtos.Response
{
    public class ChannelResponseDto
    {
        public long ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public List<ChannelAthletesDto> ChannelAthletes { get; set; } = new List<ChannelAthletesDto>();
        public string LastMessage { get; set; } = string.Empty;
    }
}
