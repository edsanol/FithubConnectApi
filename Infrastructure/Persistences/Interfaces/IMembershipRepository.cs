using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IMembershipRepository
    {
        Task<BaseEntityResponse<Membership>> ListMemberships(BaseFiltersRequest filters);
        Task<IEnumerable<Membership>> ListSelectMemberships(int gymID);
        Task<Membership> GetMembershipById(int membershipID);
        Task<bool> CreateMembership(Membership membership);
        Task<bool> UpdateMembership(Membership membership);
        Task<bool> DeleteMembership(int membershipID);
        Task<int> GetAthletesByMembership(int membershipID);
        Task<IEnumerable<DashboardPieResponseDto>> MembershipPercentage(int gymID);
    }
}
