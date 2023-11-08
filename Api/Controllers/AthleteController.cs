using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthleteController : ControllerBase
    {
        private readonly IAthleteApplication _athleteApplication;

        public AthleteController(IAthleteApplication athleteApplication)
        {
            _athleteApplication = athleteApplication;
        }

        [HttpPost("List")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<AthleteResponseDto>>>> ListAthletes([FromBody] BaseFiltersRequest filters)
        {
            var response = await _athleteApplication.ListAthletes(filters);

            return Ok(response);
        }

        [HttpGet("{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<AthleteResponseDto>>> AthleteById(int athleteId)
        {
            var response = await _athleteApplication.AthleteById(athleteId);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterAthlete([FromBody] AthleteRequestDto request)
        {
            var response = await _athleteApplication.RegisterAthlete(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("Edit/{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditAthlete(int athleteId, [FromBody] AthleteRequestDto request)
        {
            var response = await _athleteApplication.EditAthlete(athleteId, request);

            return Ok(response);
        }

        [HttpPut("Delete/{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteAthlete(int athleteId)
        {
            var response = await _athleteApplication.RemoveAthlete(athleteId);

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<BaseResponse<AthleteResponseDto>>> LoginAthlete([FromBody] LoginRequestDto request)
        {
            var response = await _athleteApplication.LoginAthlete(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
