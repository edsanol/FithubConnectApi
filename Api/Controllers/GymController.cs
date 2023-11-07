using Application.Dtos.Request;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
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
        public async Task<IActionResult> ListGyms([FromBody] BaseFiltersRequest filters)
        {
            var response = await _gymApplication.ListGyms(filters);

            return Ok(response);
        }

        [HttpGet("Select")]
        public async Task<IActionResult> ListGymsSelect()
        {
            var response = await _gymApplication.ListGymsSelect();

            return Ok(response);
        }

        [HttpGet("{gymId:int}")]
        public async Task<IActionResult> GymById(int gymId)
        {
            var response = await _gymApplication.GymById(gymId);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterGym([FromBody] GymRequestDto request)
        {
            var response = await _gymApplication.RegisterGym(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("Edit/{gymId:int}")]
        public async Task<IActionResult> EditGym(int gymId, [FromBody] GymRequestDto request)
        {
            var response = await _gymApplication.EditGym(gymId, request);

            return Ok(response);
        }

        [HttpPut("Delete/{gymId:int}")]
        public async Task<IActionResult> RemoveGym(int gymId)
        {
            var response = await _gymApplication.RemoveGym(gymId);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginGym([FromBody] LoginRequestDto login)
        {
            var response = await _gymApplication.LoginGym(login);

            if (response.IsSuccess == false)
                return Unauthorized();

            return Ok(response);
        }
    }
}
