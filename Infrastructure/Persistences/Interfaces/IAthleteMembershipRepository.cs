using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAthleteMembershipRepository
    {
        Task<bool> RegisterAthleteMembership(AthleteMembership athleteMembership);
    }
}
