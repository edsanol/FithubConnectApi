using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IContactInformationRepository
    {
        Task<ContactInformation> GetContactInformation();
    }
}
