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
        Task<BaseResponse<AthleteEditResponseDto>> EditAthleteMobile(AthleteEditRequestDto athleteDto);
        Task<BaseResponse<bool>> RemoveAthlete(int athleteID);
        Task<BaseResponse<AthleteResponseDto>> LoginAthlete(LoginRequestDto loginDto);
        Task<BaseResponse<bool>> UpdateMembershipToAthlete(MembershipToAthleteRequestDto membershipToAthleteDto);
        Task<bool> AccessAthlete(string accessAthleteDto);
        Task<BaseResponse<int>> VerifyAccessAthlete(VerifyAccessRequestDto verifyAccessDto);
        Task<BaseResponse<AthleteResponseDto>> RegisterPassword(LoginRequestDto loginRequestDto);
        Task<BaseResponse<bool>> RecordMeasurementProgress(MeasurementProgressRequestDto measurementProgressDto);
        Task<BaseResponse<BaseEntityResponse<MeasurementProgressResponseDto>>> GetMeasurementProgressList(BaseFiltersRequest filters, int athleteID);
        Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetMeasurementsGraphic(string muscle, DateOnly startDate, DateOnly endDate, int athleteID);
        Task<BaseResponse<IEnumerable<MeasurementsByLastMonthResponseDto>>> GetMeasurementsByLastMonth(int athleteID);
        Task<BaseResponse<AthleteResponseDto>> RefreshAuthToken(string refreshToken);
        Task<BaseResponse<ContactInformationResponseDto>> GetContactInformation();
        Task<BaseResponse<bool>> RegisterAthleteFingerPrint(FingerprintRequest request);
        Task<BaseResponse<bool>> AccessAthleteFingerPrint(int athleteID);
    }
}
