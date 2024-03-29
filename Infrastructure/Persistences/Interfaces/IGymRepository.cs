using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGymRepository
    {
        Task<BaseEntityResponse<Gym>> ListGyms(BaseFiltersRequest filters);
        Task<IEnumerable<Gym>> ListSelectGyms();
        Task<Gym> GetGymById(int gymID);
        Task<bool> RegisterGym(Gym gym);
        Task<bool> EditGym(Gym gym);
        Task<bool> DeleteGym(int gymID);
        Task<Gym> LoginGym(string email);
        Task<bool> ResetPasswordAsync(int gymId, string newPassword);
        Task<bool> ChangePasswordAsync(int gymId, string newPassword);
        Task<bool> HasAthleteByAthleteID(int gymID, int athleteID);
    }
}
