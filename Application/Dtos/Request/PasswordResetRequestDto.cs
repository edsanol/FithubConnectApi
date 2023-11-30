namespace Application.Dtos.Request
{
    public class PasswordResetRequestDto
    {
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
