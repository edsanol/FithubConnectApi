using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersPaymentsController : ControllerBase
    {
        private readonly IOrdersPaymentsApplication _ordersPaymentsApplication;

        public OrdersPaymentsController(IOrdersPaymentsApplication ordersPaymentsApplication)
        {
            _ordersPaymentsApplication = ordersPaymentsApplication;
        }

        [Authorize]
        [HttpPost("RegisterOrder")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterOrder([FromBody] OrderRequestDto request)
        {
            var response = await _ordersPaymentsApplication.RegisterOrder(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
