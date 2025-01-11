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
        Task<BaseResponse<BaseEntityResponse<ExercisesResponseDto>>> GetExercisesList(BaseFiltersRequest filters);
        Task<BaseResponse<bool>> UpdateExercise(UpdateExerciseRequestDto updateExerciseRequestDto);
        Task<BaseResponse<bool>> DeleteExercise(long exerciseId);
        Task<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>> GetRoutinesByAthleteIdList(BaseFiltersRequest filters, int athleteId);
        Task<BaseResponse<bool>> UpdateRoutine(UpdateRoutineRequestDto updateRoutineRequestDto);
    }
}
