﻿using Api.Attributes;
using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Domain.Entities;
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

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false && response.Message == "No se encontraron registros")
                return NotFound(response);

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

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("EditMobile")]
        public async Task<ActionResult<BaseResponse<AthleteEditResponseDto>>> EditAthleteMobile([FromBody] AthleteEditRequestDto request)
        {
            var response = await _athleteApplication.EditAthleteMobile(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("Delete/{athleteId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteAthlete(int athleteId)
        {
            var response = await _athleteApplication.RemoveAthlete(athleteId);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

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

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("AccessAthleteCard")]
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

        [Authorize]
        [HttpPost("RecordMeasurementProgress")]
        public async Task<ActionResult<bool>> RecordMeasurementProgress([FromBody] MeasurementProgressRequestDto measurementProgressDto)
        {
            var response = await _athleteApplication.RecordMeasurementProgress(measurementProgressDto);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("GetMeasurementProgressList")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<MeasurementProgressResponseDto>>>> 
            GetMeasurementProgressList([FromBody] BaseFiltersRequest filters, int athleteID = 0)
        {
            var response = await _athleteApplication.GetMeasurementProgressList(filters, athleteID);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetMeasurementsGraphic")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>>>
            GetMeasurementsGraphic([FromQuery] string muscle, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, [FromQuery] int athleteID = 0)
        {
            var response = await _athleteApplication.GetMeasurementsGraphic(muscle, startDate, endDate, athleteID);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetMeasurementsByLastMonth")]
        public async Task<ActionResult<BaseResponse<IEnumerable<MeasurementsByLastMonthResponseDto>>>> GetMeasurementsByLastMonth([FromQuery] int athleteID = 0)
        {
            var response = await _athleteApplication.GetMeasurementsByLastMonth(athleteID);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [ValidateRefreshToken]
        [Route("refreshToken")]
        public async Task<ActionResult<BaseResponse<AthleteResponseDto>>> RefreshAuthToken([FromHeader(Name = "RefreshToken")] string refreshToken)
        {
            var response = await _athleteApplication.RefreshAuthToken(refreshToken);

            if (response.IsSuccess == false)
                return Unauthorized();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetContactInformation")]
        public async Task<ActionResult<BaseResponse<ContactInformationResponseDto>>> GetContactInformation()
        {
            var response = await _athleteApplication.GetContactInformation();

            return Ok(response);
        }

        [Authorize]
        [HttpPost("assign")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterAthleteFingerPrint([FromBody] FingerprintRequest request)
        {
            var response = await _athleteApplication.RegisterAthleteFingerPrint(request);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("AccessAthleteFingerPrint")]
        public async Task<ActionResult<BaseResponse<bool>>> AccessAthleteFingerPrint([FromQuery] int athleteID, int? accessType)
        {
            var response = await _athleteApplication.AccessAthleteFingerPrint(athleteID, accessType);

            return Ok(response);
        }

        [HttpDelete("DestroyAthleteFromDB")]
        public async Task<ActionResult<BaseResponse<bool>>> DestroyAthleteFromDB([FromBody] DestroyAthleteRequestDto request)
        {
            var response = await _athleteApplication.DestroyAthleteFromDB(request);

            return Ok(response);
        }

        [HttpPost("RegisterAthleteByQR/{gymID}")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterAthleteByQR([FromRoute] string gymID, [FromBody] AthleteRequestDto request)
        {
            var response = await _athleteApplication.RegisterAthleteByQR(gymID, request);

            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("RegisterDeviceToken")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterDeviceToken([FromBody] DeviceTokenRequestDto request)
        {
            var response = await _athleteApplication.RegisterDeviceToken(request);

            if (response.IsSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
