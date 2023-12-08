using Domain.Entities;

namespace Application.Interfaces
{
    public interface IJwtHandler
    {
        Task<string> GenerateToken(Gym gym);
        Task<string> GeneratePasswordResetToken(int userId);
        bool ValidateToken(string token);
        int ExtractGymIdFromToken();
    }
}
