using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IAthleteApplication
    {
        Task<BaseResponse<BaseEntityResponse<AthleteResponseDto>>> ListAthletes(BaseFiltersRequest filters);
        Task<BaseResponse<AthleteResponseDto>> AthleteById(int athleteID);
        Task<BaseResponse<bool>> RegisterAthlete(AthleteRequestDto athleteDto);
        Task<BaseResponse<bool>> EditAthlete(int athleteID, AthleteRequestDto athleteDto);
        Task<BaseResponse<bool>> RemoveAthlete(int athleteID);
        Task<BaseResponse<AthleteResponseDto>> LoginAthlete(LoginRequestDto loginDto);
        Task<BaseResponse<bool>> UpdateMembershipToAthlete(MembershipToAthleteRequestDto membershipToAthleteDto);
    }
}
