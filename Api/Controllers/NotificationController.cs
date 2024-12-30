using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationApplication _notificationApplication;

        public NotificationController(INotificationApplication notificationApplication)
        {
            _notificationApplication = notificationApplication;
        }

        [HttpPost("CreateChannel")]
        public async Task<ActionResult<BaseResponse<int>>> CreateChannel([FromBody] ChannelRequestDto channelRequestDto)
        {
            var response = await _notificationApplication.CreateChannel(channelRequestDto);

            return Ok(response);
        }

        [HttpPost("AddUserToChannel")]
        public async Task<ActionResult<BaseResponse<int>>> AddUserToChannel([FromBody] UserChannelRequestDto userChannelRequestDto)
        {
            var response = await _notificationApplication.AddUserToChannel(userChannelRequestDto);

            return Ok(response);
        }

        [HttpPost("RemoveUserFromChannel")]
        public async Task<ActionResult<BaseResponse<int>>> RemoveUserFromChannel([FromBody] UserChannelRequestDto userChannelRequestDto)
        {
            var response = await _notificationApplication.RemoveUserFromChannel(userChannelRequestDto);

            return Ok(response);
        }

        [HttpGet("GetChannels")]
        public async Task<ActionResult<BaseResponse<List<ChannelResponseDto>>>> GetChannels()
        {
            var response = await _notificationApplication.GetChannels();

            return Ok(response);
        }
    }
}
