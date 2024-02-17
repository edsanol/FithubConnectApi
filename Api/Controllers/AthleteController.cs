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
    public class AthleteController : ControllerBase
    {
        private readonly IAthleteApplication _athleteApplication;

        public AthleteController(IAthleteApplication athleteApplication)
        {
            _athleteApplication = athleteApplication;
        }

        [Authorize]
        [HttpPost("List")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<AthleteResponseDto>>>> ListAthletes([FromBody] BaseFiltersRequest filters)
        {
            var response = await _athleteApplication.ListAthletes(filters);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<AthleteResponseDto>>> AthleteById(int athleteId)
        {
            var response = await _athleteApplication.AthleteById(athleteId);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("Register")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterAthlete([FromBody] AthleteRequestDto request)
        {
            var response = await _athleteApplication.RegisterAthlete(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("Edit/{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditAthlete(int athleteId, [FromBody] AthleteRequestDto request)
        {
            var response = await _athleteApplication.EditAthlete(athleteId, request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
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

        [Authorize]
        [HttpPost("UpdateMembershipToAthlete")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateMembershipToAthlete([FromBody] MembershipToAthleteRequestDto request)
        {
            var response = await _athleteApplication.UpdateMembershipToAthlete(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("AccessAthlete")]
        public async Task<ActionResult<bool>> AccessAthlete(string request)
        {
            var response = await _athleteApplication.AccessAthlete(request);

            if (response == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("VerifyLogin")]
        public async Task<ActionResult<BaseResponse<int>>> VerifyAccessAthlete([FromBody] VerifyAccessRequestDto request)
        {
            var response = await _athleteApplication.VerifyAccessAthlete(request);
            return Ok(response);
        }

        [HttpPost("CreatePassword")]
        public async Task<ActionResult<BaseResponse<AthleteResponseDto>>> RegisterPassword([FromBody] LoginRequestDto loginRequestDto)
        {
            var response = await _athleteApplication.RegisterPassword(loginRequestDto);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("RecordMeasurementProgress")]
        public async Task<ActionResult<bool>> RecordMeasurementProgress([FromBody] MeasurementProgressRequestDto measurementProgressDto)
        {
            var response = await _athleteApplication.RecordMeasurementProgress(measurementProgressDto);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("GetMeasurementProgressList")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<MeasurementProgressResponseDto>>>> 
            GetMeasurementProgressList([FromBody] BaseFiltersRequest filters, int athleteID)
        {
            var response = await _athleteApplication.GetMeasurementProgressList(filters, athleteID);

            return Ok(response);
        }

        [HttpGet("GetMeasurementsGraphic")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>>>
            GetMeasurementsGraphic(int athleteID, [FromQuery] string muscle, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            var response = await _athleteApplication.GetMeasurementsGraphic(athleteID, muscle, startDate, endDate);

            return Ok(response);
        }

        [HttpGet("GetMeasurementsByLastMonth")]
        public async Task<ActionResult<BaseResponse<IEnumerable<MeasurementsByLastMonthResponseDto>>>> GetMeasurementsByLastMonth(int athleteID)
        {
            var response = await _athleteApplication.GetMeasurementsByLastMonth(athleteID);

            return Ok(response);
        }
    }
}
