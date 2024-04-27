using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAthleteTokenRepository
    {
        Task<bool> RegisterAthleteToken(AthleteToken athleteToken);
        Task<int[]> GetAthleteToken(int athleteId);
        Task<bool> RevokeAthleteToken(string actualToken);
        Task<bool> GetRevokeStatus(string token);
    }
}
