using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IMeasurementProgressRepository
    {
        Task<bool> RecordMeasurementProgress(MeasurementsProgress measurementProgress);
        Task<BaseEntityResponse<MeasurementsProgress>> GetMeasurementProgressList(BaseFiltersRequest filters, int athleteID);
        Task<IEnumerable<DashboardGraphicsResponse>> GetMeasurementsGraphic(int athleteID, string muscle, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<MeasurementsByLastMonthResponse>> GetMeasurementsByLastMonth(int athleteID);
    }
}
