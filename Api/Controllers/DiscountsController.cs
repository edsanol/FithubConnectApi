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
    [Authorize]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountApplication _discountApplication;

        public DiscountsController(IDiscountApplication discountApplication)
        {
            _discountApplication = discountApplication;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<DiscountResponseDto>>>> ListDiscounts([FromBody] BaseFiltersRequest filters)
        {
            var response = await _discountApplication.ListDiscounts(filters);

            return Ok(response);
        }

        [HttpGet("Select/{gymId:int}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<DiscountSelectResponseDto>>>> ListDiscountsSelect(int gymId)
        {
            var response = await _discountApplication.ListDiscountsSelect(gymId);

            return Ok(response);
        }

        [HttpGet("{discountId:int}")]
        public async Task<ActionResult<BaseResponse<DiscountResponseDto>>> DiscountById(int discountId)
        {
            var response = await _discountApplication.DiscountById(discountId);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterDiscount([FromBody] DiscountRequestDto request)
        {
            var response = await _discountApplication.CreateDiscount(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("Edit/{discountId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditDiscount(int discountId, [FromBody] DiscountRequestDto request)
        {
            var response = await _discountApplication.UpdateDiscount(discountId, request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("Delete/{discountId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> RemoveDiscount(int discountId)
        {
            var response = await _discountApplication.DeleteDiscount(discountId);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
