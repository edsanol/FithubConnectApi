namespace Application.Dtos.Request
{
    public class SendRoutineToChannelRequestDto
    {
        public long RoutineId { get; set; }
        public long ChannelId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
