using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ICardAccessRepository
    {
        Task<bool> RegisterCardAccess(CardAccess cardAccess);
        Task<bool> UnregisterCardAccess(CardAccess cardAccess);
        Task<bool> GetActiveAccessByCode(string cardCode);
        Task<CardAccess>? GetAccessCardByCode(string cardCode);
    }
}
