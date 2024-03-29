using Domain.Entities;

namespace Application.Interfaces
{
    public interface IJwtHandler
    {
        Task<string> GenerateToken(Gym gym);
        Task<string> GenerateRefreshToken(Gym gym);
        Task<string> GenerateAthleteToken(Athlete gym);
        Task<string> GenerateAthleteRefreshToken(Athlete gym);
        Task<string> GeneratePasswordResetToken(int userId);
        bool ValidateToken(string token);
        int GetIdFromRefreshToken(string refreshToken);
        string GetRoleFromRefreshToken(string token);
        string GetTokenTypeFromRefreshToken(string token);
        int ExtractIdFromToken();
        string GetRoleFromToken();
    }
}
