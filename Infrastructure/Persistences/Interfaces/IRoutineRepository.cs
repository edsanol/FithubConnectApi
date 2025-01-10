using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineRepository
    {
        Task<bool> CreateRoutine(Routines routine);
    }
}
