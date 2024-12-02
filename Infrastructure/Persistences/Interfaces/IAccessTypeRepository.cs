using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAccessTypeRepository
    {
        Task<IEnumerable<AccessType>> ListAccessTypes();
    }
}
