namespace Application.Dtos.Response
{
    public class ChannelResponseDto
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public List<ChannelAthletesDto> ChannelAthletes { get; set; } = new List<ChannelAthletesDto>();
    }
}
