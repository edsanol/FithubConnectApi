using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IOrdersPaymentsRepository
    {
        Task<bool> RegisterOrder(Orders order);
    }
}
