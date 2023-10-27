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
        Task<BaseResponse<bool>> EditGym(int gymID, GymRequestDto gymDto);
        Task<BaseResponse<bool>> RemoveGym(int gymID);
        Task<BaseResponse<GymResponseDto>> LoginGym(LoginRequestDto loginDto);
    }
}
