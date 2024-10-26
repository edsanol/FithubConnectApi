using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAccessLogRepository
    {
        Task<bool> RegisterAccessLog(AccessLog accessLog);
        Task<BaseEntityResponse<AthleteAssistenceDto>> GetAthleteAssitance(BaseFiltersRequest filters, int gymID);
    }
}
