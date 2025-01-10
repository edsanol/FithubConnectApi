namespace Application.Dtos.Request
{
    public class NewExerciseRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
    }
}
