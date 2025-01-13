using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineExerciseRepository
    {
        Task<bool> CreateRoutineExercise(RoutineExercises routineExercise);
        Task<bool> DeleteExercisesFromRoutine(long routineId, List<long> DeleteExercises);
        Task<bool> IsExerciseInRoutine(long routineId, long exerciseId);
        Task<RoutineExercises?> GetRoutineExerciseByRoutineAndExercise(long routineId, long exerciseId);
        Task<RoutineExercises?> GetRoutineExerciseById(long routineExerciseId);
    }
}
