using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IRoutineApplication
    {
        Task<BaseResponse<bool>> CreateRoutine(CreateRoutineRequestDto createRoutineRequestDto);
        Task<BaseResponse<bool>> CreateExercise(NewExerciseRequestDto createExerciseRequestDto);
        Task<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>> GetRoutinesList(BaseFiltersRequest filters);
    }
}
