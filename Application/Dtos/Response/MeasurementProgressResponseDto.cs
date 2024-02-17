namespace Application.Dtos.Response
{
    public class MeasurementProgressResponseDto
    {
        public int MeasurementsProgressID { get; set; }

        public int IdAthlete { get; set; }

        public decimal? Gluteus { get; set; }

        public decimal? Biceps { get; set; }

        public decimal? Chest { get; set; }

        public decimal? Waist { get; set; }

        public decimal? Thigh { get; set; }

        public decimal? Calf { get; set; }

        public decimal? Shoulders { get; set; }

        public decimal? Forearm { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public DateOnly? Date { get; set; }
    }
}
