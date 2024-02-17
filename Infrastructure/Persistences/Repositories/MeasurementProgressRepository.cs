using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class MeasurementProgressRepository : GenericRepository<MeasurementsProgress>, IMeasurementProgressRepository
    {
        private readonly DbFithubContext _context;

        public MeasurementProgressRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<DashboardGraphicsResponse>> GetGluteusGraphic(int athleteID, DateOnly startDate, DateOnly endDate)
        {
            var query = _context.MeasurementsProgress.Where(x => x.IdAthlete == athleteID && x.Date >= startDate && x.Date <= endDate)
                .AsNoTracking().AsQueryable();

            return await query.Select(x => new DashboardGraphicsResponse
            {
                Time = (DateOnly)x.Date,
                Value = (float)x.Gluteus
            }).ToListAsync();
        }

        public async Task<BaseEntityResponse<MeasurementsProgress>> GetMeasurementProgressList(BaseFiltersRequest filters, int athleteID)
        {
            var response = new BaseEntityResponse<MeasurementsProgress>();
            var query = _context.MeasurementsProgress.Where(x => x.IdAthlete == athleteID)
                .AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                query = query.Where(x => x.Date == DateOnly.FromDateTime(Convert.ToDateTime(filters.StartDate)));
            }

            filters.Sort ??= "Date";

            response.TotalRecords = await query.CountAsync();
            response.Items = await Ordering(filters, query, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<bool> RecordMeasurementProgress(MeasurementsProgress measurementProgress)
        {
            await _context.MeasurementsProgress.AddAsync(measurementProgress);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
