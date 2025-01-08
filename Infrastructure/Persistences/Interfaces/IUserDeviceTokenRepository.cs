using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUserDeviceTokenRepository
    {
        Task<bool> SaveUserDeviceToken(UserDeviceToken userDeviceToken);
        Task<bool> ExistsUserDeviceToken(string token);
        Task<List<string>> GetDeviceTokensByAthleteIds(List<int> athleteIds);
    }
}
