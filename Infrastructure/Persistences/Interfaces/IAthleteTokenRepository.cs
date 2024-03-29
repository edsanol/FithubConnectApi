using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAthleteTokenRepository
    {
        Task<bool> RegisterAthleteToken(AthleteToken athleteToken);
        Task<int[]> GetAthleteToken(int athleteId);
        Task<bool> RevokeAthleteToken(int[] tokenID);
        Task<bool> GetRevokeStatus(string token);
    }
}
