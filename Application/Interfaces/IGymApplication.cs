using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IGymApplication
    {
        Task<BaseResponse<BaseEntityResponse<GymResponseDto>>> ListGyms(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<GymSelectResponseDto>>> ListGymsSelect();
        Task<BaseResponse<GymResponseDto>> GymById(int gymID);
        Task<BaseResponse<bool>> RegisterGym(GymRequestDto gymDto);
        Task<BaseResponse<bool>> EditGym(GymRequestDto gymDto);
        Task<BaseResponse<bool>> RemoveGym();
        Task<BaseResponse<GymResponseDto>> LoginGym(LoginRequestDto loginDto);
        Task<BaseResponse<GymResponseDto>> RefreshAuthToken(string refreshToken);
        Task<BaseResponse<bool>> RecoverPassword(RecoverPasswordRequestDto request);
        Task<BaseResponse<bool>> ResetPassword(PasswordResetRequestDto request);
        Task<BaseResponse<bool>> ChangePassword(ChangePasswordRequestDto request);
        Task<BaseResponse<IEnumerable<AccessTypeResponseDto>>> ListAccessTypes();
    }
}
