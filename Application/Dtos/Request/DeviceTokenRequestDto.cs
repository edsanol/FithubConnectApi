namespace Application.Dtos.Request
{
    public class DeviceTokenRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public string DeviceBrand { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
    }
}
