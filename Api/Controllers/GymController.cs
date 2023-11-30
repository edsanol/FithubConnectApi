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
    public class GymController : ControllerBase
    {
        private readonly IGymApplication _gymApplication;

        public GymController(IGymApplication gymApplication)
        {
            _gymApplication = gymApplication;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<GymResponseDto>>>> ListGyms([FromBody] BaseFiltersRequest filters)
        {
            var response = await _gymApplication.ListGyms(filters);

            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GymSelectResponseDto>>>> ListGymsSelect()
        {
            var response = await _gymApplication.ListGymsSelect();

            return Ok(response);
        }

        [HttpGet("{gymId:int}")]
        public async Task<ActionResult<BaseResponse<GymResponseDto>>> GymById(int gymId)
        {
            var response = await _gymApplication.GymById(gymId);

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

        [HttpPut("Edit/{gymId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditGym(int gymId, [FromBody] GymRequestDto request)
        {
            var response = await _gymApplication.EditGym(gymId, request);

            return Ok(response);
        }

        [HttpPut("Delete/{gymId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> RemoveGym(int gymId)
        {
            var response = await _gymApplication.RemoveGym(gymId);

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
    }
}
