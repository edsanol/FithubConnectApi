using Domain.Entities;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AthleteMembershipRepository : GenericRepository<AthleteMembership>, IAthleteMembershipRepository
    {
        private readonly DbFithubContext _context;

        public AthleteMembershipRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RegisterAthleteMembership(AthleteMembership athleteMembership)
        {
            await _context.AthleteMemberships.AddAsync(athleteMembership);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<IEnumerable<DashboardGraphicsResponseDto>> GetIncome(int gymID, DateOnly startDate, DateOnly endDate)
        {
            var income = await _context.AthleteMemberships
                .Where(x => x.IdAthleteNavigation.IdGym.Equals(gymID) && x.StartDate >= startDate && x.StartDate <= endDate)
                .GroupBy(x => x.StartDate)
                .Select(x => new DashboardGraphicsResponseDto
                {
                    Time = x.Key,
                    Value = (float)x.Sum(am => am.IdMembershipNavigation.Cost)
                })
                .ToListAsync();

            return income;
        }
    }
}
