using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IMeasurementProgressRepository
    {
        Task<bool> RecordMeasurementProgress(MeasurementsProgress measurementProgress);
    }
}
