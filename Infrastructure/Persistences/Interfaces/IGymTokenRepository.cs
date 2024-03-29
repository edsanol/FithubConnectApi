using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGymTokenRepository
    {
        Task<bool> RegisterGymToken(GymToken gymToken);
        Task<int[]> GetGymToken(int gymId);
        Task<bool> RevokeGymToken(int[] tokenID);
        Task<bool> GetRevokeStatus(string token);
    }
}
