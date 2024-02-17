using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class MeasurementProgressRepository : GenericRepository<MeasurementsProgress>, IMeasurementProgressRepository
    {
        private readonly DbFithubContext _context;

        public MeasurementProgressRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RecordMeasurementProgress(MeasurementsProgress measurementProgress)
        {
            await _context.MeasurementsProgress.AddAsync(measurementProgress);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
