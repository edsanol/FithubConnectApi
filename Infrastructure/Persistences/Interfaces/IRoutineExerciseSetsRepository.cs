using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineExerciseSetsRepository
    {
        Task<bool> CreateRoutineExerciseSets(RoutineExerciseSets routineExerciseSets);
        Task<bool> DeleteSets(List<long> DeleteSets);
        Task<bool> DeleteSetsByExercises(List<long> DeleteExercises);
    }
}
