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

        public async Task<IEnumerable<DashboardGraphicsResponse>> GetMeasurementsGraphic(int athleteID, string muscle, DateOnly startDate, DateOnly endDate)
        {
            var muscleSelector = BuildMuscleSelector(muscle);

            var query = _context.MeasurementsProgress
                .Where(x => x.IdAthlete == athleteID && x.Date >= startDate && x.Date <= endDate)
                .AsNoTracking()
                .Select(x => new DashboardGraphicsResponse
                {
                    Time = (DateOnly)x.Date,
                    Value = muscleSelector(x)
                });

            return await query.ToListAsync();
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

        public async Task<IEnumerable<MeasurementsByLastMonthResponse>> GetMeasurementsByLastMonth(int athleteID)
        {
            var measurements = await _context.MeasurementsProgress
                .Where(x => x.IdAthlete == athleteID)
                .OrderByDescending(x => x.Date)
                .Take(2)
                .AsNoTracking()
                .ToListAsync();

            if (measurements.Count < 2) return Enumerable.Empty<MeasurementsByLastMonthResponse>();

            var lastMeasurement = measurements[0];
            var previousMeasurement = measurements[1];

            var result = new List<MeasurementsByLastMonthResponse>();

            foreach (var muscle in new[] { "Gluteus", "Biceps", "Chest", "Waist", "Thigh", "Calf", "Shoulders", "Forearm", "Height", "Weight" })
            {
                var lastValue = GetMuscleValue(lastMeasurement, muscle);
                var previousValue = GetMuscleValue(previousMeasurement, muscle);

                if (lastValue.HasValue && previousValue.HasValue && previousValue.Value != 0)
                {
                    var progress = lastValue.Value - previousValue.Value;
                    var progressPercentage = (progress / previousValue.Value) * 100;

                    result.Add(new MeasurementsByLastMonthResponse
                    {
                        Muscle = muscle,
                        Progress = progress,
                        Measurement = lastValue.Value,
                        ProgressPercentage = progressPercentage
                    });
                }
            }

            return result;
        }

        private static Func<MeasurementsProgress, float> BuildMuscleSelector(string muscle)
        {
            return muscle.ToLower() switch
            {
                "gluteus" => x => (float)x.Gluteus,
                "biceps" => x => (float)x.Biceps,
                "chest" => x => (float)x.Chest,
                "waist" => x => (float)x.Waist,
                "thigh" => x => (float)x.Thigh,
                "calf" => x => (float)x.Calf,
                "shoulder" => x => (float)x.Shoulders,
                "forearm" => x => (float)x.Forearm,
                "height" => x => (float)x.Height,
                "weight" => x => (float)x.Weight,
                _ => throw new ArgumentException("Invalid muscle name", nameof(muscle)),
            };
        }

        private float? GetMuscleValue(MeasurementsProgress measurement, string muscle)
        {
            // Aquí deberás implementar la lógica para devolver el valor correcto basado en el nombre del músculo.
            // Esta implementación es un ejemplo y debe ser ajustada según la estructura de tu entidad.
            switch (muscle.ToLower())
            {
                case "gluteus": return (float?)measurement.Gluteus;
                case "biceps": return (float?)measurement.Biceps;
                case "chest": return (float?)measurement.Chest;
                case "waist": return (float?)measurement.Waist;
                case "thigh": return (float?)measurement.Thigh;
                case "calf": return (float?)measurement.Calf;
                case "shoulders": return (float?)measurement.Shoulders;
                case "forearm": return (float?)measurement.Forearm;
                case "height": return (float?)measurement.Height;
                case "weight": return (float?)measurement.Weight;
                default: return null;
            }
        }
    }
}
