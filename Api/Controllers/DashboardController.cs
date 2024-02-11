using Application.Commons.Bases;
using Application.Dtos.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardApplication _dashboardApplication;

        public DashboardController(IDashboardApplication dashboardApplication)
        {
            _dashboardApplication = dashboardApplication;
        }

        [HttpGet("GetDashboard")]
        public async Task<ActionResult<BaseResponse<DashboardAthleteResponseDto>>> GetDashboard()
        {
            var response = await _dashboardApplication.GetDashboard();

            return Ok(response);
        }

        [HttpGet("GetDailyAssistanceGraphic")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>>> GetDailyAssistanceGraphic([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            // if startDate and endDate are not provided, set them to today
            if (startDate == default)
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

            if (endDate == default)
                endDate = DateOnly.FromDateTime(DateTime.Now);

            var response = await _dashboardApplication.GetDailyAssistanceGraphic(startDate, endDate);

            return Ok(response);
        }

        [HttpGet("GetIncomeGraphic")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>>> GetIncomeGraphic([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            // if startDate and endDate are not provided, set them to today
            if (startDate == default)
                startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

            if (endDate == default)
                endDate = DateOnly.FromDateTime(DateTime.Now);

            var response = await _dashboardApplication.GetIncomeGraphic(startDate, endDate);

            return Ok(response);
        }

        [HttpGet("GetMembershipGraphic")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DashboardPieResponseDto>>>> GetMembershipPercentage()
        {
            var response = await _dashboardApplication.GetMembershipPercentage();

            return Ok(response);
        }
    }
}
