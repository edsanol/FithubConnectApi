using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Interfaces;
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
    }
}
