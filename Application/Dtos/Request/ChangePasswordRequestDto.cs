namespace Application.Dtos.Request
{
    public class ChangePasswordRequestDto
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
    }
}
