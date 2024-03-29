using Api.Attributes;
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
    public class GymController : ControllerBase
    {
        private readonly IGymApplication _gymApplication;

        public GymController(IGymApplication gymApplication)
        {
            _gymApplication = gymApplication;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<GymResponseDto>>>> ListGyms([FromBody] BaseFiltersRequest filters)
        {
            var response = await _gymApplication.ListGyms(filters);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("Select")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GymSelectResponseDto>>>> ListGymsSelect()
        {
            var response = await _gymApplication.ListGymsSelect();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GymById")]
        public async Task<ActionResult<BaseResponse<GymResponseDto>>> GymById([FromQuery] int gymID = 0)
        {
            var response = await _gymApplication.GymById(gymID);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterGym([FromBody] GymRequestDto request)
        {
            var response = await _gymApplication.RegisterGym(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("Edit")]
        public async Task<ActionResult<BaseResponse<bool>>> EditGym([FromBody] GymRequestDto request)
        {
            var response = await _gymApplication.EditGym(request);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("Delete")]
        public async Task<ActionResult<BaseResponse<bool>>> RemoveGym()
        {
            var response = await _gymApplication.RemoveGym();

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<GymResponseDto>>> LoginGym([FromBody] LoginRequestDto login)
        {
            var response = await _gymApplication.LoginGym(login);

            if (response.IsSuccess == false)
                return Unauthorized();

            return Ok(response);
        }

        [HttpPost("RecoverPassword")]
        public async Task<ActionResult<BaseResponse<bool>>> RecoverPassword([FromBody] RecoverPasswordRequestDto request)
        {
            var response = await _gymApplication.RecoverPassword(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<BaseResponse<bool>>> ResetPassword([FromBody] PasswordResetRequestDto request)
        {
            var response = await _gymApplication.ResetPassword(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<BaseResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var response = await _gymApplication.ChangePassword(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [ValidateRefreshToken]
        [Route("refreshToken")]
        public async Task<ActionResult<BaseResponse<GymResponseDto>>> RefreshAuthToken([FromHeader(Name = "RefreshToken")] string refreshToken)
        {
            var response = await _gymApplication.RefreshAuthToken(refreshToken);

            if (response.IsSuccess == false)
                return Unauthorized();

            return Ok(response);
        }
    }
}
