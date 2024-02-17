namespace Infrastructure.Commons.Bases.Response
{
    public class MeasurementsByLastMonthResponse
    {
        public string Muscle { get; set; } = string.Empty;
        public float Progress { get; set; }
        public float Measurement { get; set; }
        public float ProgressPercentage { get; set; }
    }
}
