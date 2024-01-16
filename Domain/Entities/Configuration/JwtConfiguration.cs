namespace Domain.Entities.Configuration
{
    public class JwtConfiguration
    {
        public string Secret { get; set; } = string.Empty;
        public int Expires { get; set; }
    }
}
