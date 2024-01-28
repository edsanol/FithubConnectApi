using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAccessLogRepository
    {
        Task<bool> RegisterAccessLog(AccessLog accessLog);
    }
}
