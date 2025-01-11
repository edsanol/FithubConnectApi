using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineApplication _routineApplication;

        public RoutineController(IRoutineApplication routineApplication)
        {
            _routineApplication = routineApplication;
        }

        [HttpPost("CreateRoutine")]
        public async Task<ActionResult<BaseResponse<bool>>> CreateRoutine([FromBody] CreateRoutineRequestDto createRoutineRequestDto)
        {
            var response = await _routineApplication.CreateRoutine(createRoutineRequestDto);

            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("CreateExercise")]
        public async Task<ActionResult<BaseResponse<bool>>> CreateExercise([FromBody] NewExerciseRequestDto createExerciseRequestDto)
        {
            var response = await _routineApplication.CreateExercise(createExerciseRequestDto);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetRoutinesList")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>>> GetRoutinesList([FromBody] BaseFiltersRequest filters)
        {
            var response = await _routineApplication.GetRoutinesList(filters);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("GetExercisesList")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<ExercisesResponseDto>>>> GetExercisesList([FromBody] BaseFiltersRequest filters)
        {
            var response = await _routineApplication.GetExercisesList(filters);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("UpdateExercise")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateExercise([FromBody] UpdateExerciseRequestDto updateExerciseRequestDto)
        {
            var response = await _routineApplication.UpdateExercise(updateExerciseRequestDto);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("DeleteExercise/{exerciseId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteExercise(int exerciseId)
        {
            var response = await _routineApplication.DeleteExercise(exerciseId);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("GetRoutinesByAthleteIdList")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<RoutinesResponseDto>>>> 
            GetRoutinesByAthleteIdList([FromBody] BaseFiltersRequest filters, [FromQuery] int athleteId = 0)
        {
            var response = await _routineApplication.GetRoutinesByAthleteIdList(filters, athleteId);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
