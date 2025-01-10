using Application.Commons.Bases;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IRoutineApplication
    {
        Task<BaseResponse<bool>> CreateRoutine(CreateRoutineRequestDto createRoutineRequestDto);
    }
}
