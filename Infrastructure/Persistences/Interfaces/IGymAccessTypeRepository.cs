using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGymAccessTypeRepository
    {
        Task<bool> CreateGymAccessType(GymAccessType gymAccessType);
        Task<IEnumerable<GymAccessType>> GetGymAccessTypesByGymID(int gymID);
        Task<bool> DeleteGymAccessType(int accessTypeID);
    }
}
