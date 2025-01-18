using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IExerciseRepository
    {
        Task<bool> CreateExercise(Exercises exercise);
        Task<Exercises> GetExerciseById(long exerciseId);
        Task<BaseEntityResponse<Exercises>> GetExercisesListByGymId(BaseFiltersRequest filters, int gymId);
        Task<bool> UpdateExercise(Exercises exercise);
        Task<bool> DeleteExercise(long exerciseId);
    }
}
