using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAthleteRoutineRepository
    {
        Task<bool> CreateAthleteRoutine(List<AthleteRoutines> athleteRoutine);
        Task<List<AthleteRoutines>> GetAssignmentsByRoutineAndAthletes(long routineId, IEnumerable<int> athleteIds);
        Task<AthleteRoutines> GetAthleteRoutineByRoutineAndAthlete(long routineId, int athleteId);
        Task<bool> UpdateAthleteRoutine(AthleteRoutines athleteRoutine);
    }
}
