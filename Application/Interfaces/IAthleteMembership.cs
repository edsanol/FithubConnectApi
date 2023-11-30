using Application.Commons.Bases;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IAthleteMembership
    {
        Task<BaseResponse<bool>> CreateAthleteMembership(AthleteMembershipDto request);
    }
}
