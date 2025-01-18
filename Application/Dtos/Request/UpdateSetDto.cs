namespace Application.Dtos.Request
{
    public class UpdateSetDto
    {
        public long? SetId { get; set; }
        public int SetNumber { get; set; }
        public int? Reps { get; set; }
        public decimal? Weight { get; set; }
    }
}
