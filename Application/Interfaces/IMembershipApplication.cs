using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IMembershipApplication
    {
        Task<BaseResponse<BaseEntityResponse<MembershipResponseDto>>> ListMemberships(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<MembershipSelectResponseDto>>> ListMembershipsSelect(int gymID);
        Task<BaseResponse<MembershipResponseDto>> MembershipById(int membershipID);
        Task<BaseResponse<bool>> CreateMembership(MembershipRequestDto membership);
        Task<BaseResponse<bool>> UpdateMembership(int membershipID, MembershipRequestDto membership);
        Task<BaseResponse<bool>> DeleteMembership(int membershipID);
    }
}
