namespace Domain.Entities
{
    public class UserDeviceToken
    {
        public long UserDeviceTokenId { get; set; }

        public int IdAthlete { get; set; }

        public string Token { get; set; } = null!;

        public string DeviceBrand { get; set; } = string.Empty;

        public string DeviceModel { get; set; } = string.Empty;

        public DateTime CreadetAt { get; set; }

        public DateTime LastUsedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Athlete IdAthleteNavigation { get; set; } = null!;
    }
}
