using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IExerciseRepository
    {
        Task<bool> CreateExercise(Exercises exercise);
        Task<Exercises> GetExerciseById(long exerciseId);
    }
}
