﻿using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AthleteRoutineRepository : GenericRepository<AthleteRoutines>, IAthleteRoutineRepository
    {
        private readonly DbFithubContext _context;

        public AthleteRoutineRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateAthleteRoutine(List<AthleteRoutines> athleteRoutine)
        {
            await _context.AthleteRoutines.AddRangeAsync(athleteRoutine);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<int>> GetAthleteIdsByRoutineId(long routineId)
        {
            return await _context.AthleteRoutines
                .Where(x => x.IdRoutine == routineId)
                .Select(x => x.IdAthlete)
                .ToListAsync();
        }

        public async Task<List<AthleteRoutines>> GetAssignmentsByRoutineAndAthletes(long routineId, IEnumerable<int> athleteIds)
        {
            return await _context.AthleteRoutines
                .Where(x => x.IdRoutine == routineId && athleteIds.Contains(x.IdAthlete))
                .ToListAsync();
        }

        public async Task<AthleteRoutines> GetAthleteRoutineByRoutineAndAthlete(long routineId, int athleteId)
        {
            return await _context.AthleteRoutines
                .FirstOrDefaultAsync(x => x.IdRoutine == routineId && x.IdAthlete == athleteId) ?? new AthleteRoutines();
        }

        public async Task<bool> UpdateAthleteRoutine(AthleteRoutines athleteRoutine)
        {
            _context.AthleteRoutines.Update(athleteRoutine);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
