using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineExerciseSetsRepository
    {
        Task<bool> CreateRoutineExerciseSets(RoutineExerciseSets routineExerciseSets);
    }
}
