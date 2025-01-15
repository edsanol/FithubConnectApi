using Api.Hubs;
using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineApplication _routineApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RoutineController(IRoutineApplication routineApplication, IHubContext<NotificationHub> hubContext)
        {
            _routineApplication = routineApplication;
            _hubContext = hubContext;
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

        [HttpPut("UpdateRoutine")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateRoutine([FromBody] UpdateRoutineRequestDto updateRoutineRequestDto)
        {
            var response = await _routineApplication.UpdateRoutine(updateRoutineRequestDto);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("DeleteRoutine/{routineId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteRoutine(int routineId)
        {
            var response = await _routineApplication.DeleteRoutine(routineId);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("GetRoutineById/{routineId:int}")]
        public async Task<ActionResult<BaseResponse<RoutinesResponseDto>>> GetRoutineById(int routineId)
        {
            var response = await _routineApplication.GetRoutineById(routineId);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("GetMuscleGroups")]
        public async Task<ActionResult<BaseResponse<List<MuscleGroupsResponseDto>>>> GetMuscleGroups()
        {
            var response = await _routineApplication.GetMuscleGroups();
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("SendRoutineToChannel")]
        public async Task<ActionResult<BaseResponse<bool>>> SendRoutineToChannel([FromBody] SendRoutineToChannelRequestDto sendRoutineToChannelRequestDto)
        {
            var response = await _routineApplication.SendRoutineToChannel(sendRoutineToChannelRequestDto);

            if (response.IsSuccess)
            {
                await _hubContext.Clients.Group(sendRoutineToChannelRequestDto.ChannelId.ToString())
                    .SendAsync("ReceiveMessage", sendRoutineToChannelRequestDto.ChannelId, "Nueva rutina");
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("DesactivateRoutineToAthlete")]
        public async Task<ActionResult<BaseResponse<bool>>> DesactivateRoutine(int routineId, int athleteId)
        {
            var response = await _routineApplication.DesactivateRoutineToAthlete(routineId, athleteId);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("InsertAthleteHistoricalSets")]
        public async Task<ActionResult<BaseResponse<bool>>> InsertAthleteHistoricalSets([FromBody] InsertAthleteHistoricalSetsRequestDto request)
        {
            var response = await _routineApplication.InsertAthleteHistoricalSets(request);
            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
