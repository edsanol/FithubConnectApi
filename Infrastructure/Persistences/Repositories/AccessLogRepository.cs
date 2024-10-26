using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AccessLogRepository : GenericRepository<AccessLog>, IAccessLogRepository
    {
        private readonly DbFithubContext _context;

        public AccessLogRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BaseEntityResponse<AthleteAssistenceDto>> GetAthleteAssitance(BaseFiltersRequest filters, int gymID)
        {
            var response = new BaseEntityResponse<AthleteAssistenceDto>();

            var assistances = _context.AccessLog
                .Include(x => x.IdAthleteNavigation)
                .Where(x => x.IdAthleteNavigation.IdGym.Equals(gymID))
                .Select(x => new AthleteAssistenceDto
                {
                    AthleteId = x.IdAthlete,
                    AthleteName = x.IdAthleteNavigation.AthleteName,
                    AthleteLastName = x.IdAthleteNavigation.AthleteLastName,
                    Email = x.IdAthleteNavigation.Email,
                    DateAssistence = DateOnly.FromDateTime(x.AccessDateTime),
                    TimeAssistence = TimeOnly.FromDateTime(x.AccessDateTime)
                }).AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                assistances = assistances.Where(x => x.DateAssistence == DateOnly.FromDateTime(DateTime.Parse(filters.StartDate)));
            }

            filters.Sort ??= "TimeAssistence";
            var orderedAssistances = Ordering(filters, assistances, !(bool)filters.Download!);

            var assistanceList = await orderedAssistances.ToListAsync();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterWords = filters.TextFilter.ToLower().Split(" ");

                switch (filters.NumFilter)
                {
                    case 1:
                        assistanceList = assistanceList
                            .Where(x =>
                                filterWords.All(word =>
                                    (x.AthleteName + " " + x.AthleteLastName).ToLower().Contains(word)
                                )).ToList();
                        break;
                }
            }

            response.TotalRecords = assistances.Count();
            response.Items = assistanceList;

            return response;
        }

        public async Task<bool> RegisterAccessLog(AccessLog accessLog)
        {
            await _context.AccessLog.AddAsync(accessLog);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
