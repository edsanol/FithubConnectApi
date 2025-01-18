using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IMuscleGroupRepository
    {
        Task<List<MuscleGroups>> GetMuscleGroups();
    }
}
