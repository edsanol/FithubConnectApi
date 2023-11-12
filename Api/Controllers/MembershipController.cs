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
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipApplication _membershipApplication;

        public MembershipController(IMembershipApplication membershipApplication)
        {
            _membershipApplication = membershipApplication;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<MembershipResponseDto>>>> ListMemberships([FromBody] BaseFiltersRequest filters)
        {
            var response = await _membershipApplication.ListMemberships(filters);

            return Ok(response);
        }

        [HttpGet("Select/{gymId:int}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<MembershipSelectResponseDto>>>> ListMembershipsSelect(int gymId)
        {
            var response = await _membershipApplication.ListMembershipsSelect(gymId);

            return Ok(response);
        }

        [HttpGet("{membershipId:int}")]
        public async Task<ActionResult<BaseResponse<MembershipResponseDto>>> MembershipById(int membershipId)
        {
            var response = await _membershipApplication.MembershipById(membershipId);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterMembership([FromBody] MembershipRequestDto request)
        {
            var response = await _membershipApplication.CreateMembership(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("Edit/{membershipId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditMembership(int membershipId, [FromBody] MembershipRequestDto request)
        {
            var response = await _membershipApplication.UpdateMembership(membershipId, request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("Delete/{membershipId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> RemoveMembership(int membershipId)
        {
            var response = await _membershipApplication.DeleteMembership(membershipId);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
