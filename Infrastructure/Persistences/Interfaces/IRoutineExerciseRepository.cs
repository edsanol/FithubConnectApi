using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineExerciseRepository
    {
        Task<bool> CreateRoutineExercise(RoutineExercises routineExercise);
    }
}
