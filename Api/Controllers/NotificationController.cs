using Api.Hubs;
using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationApplication _notificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(
            INotificationApplication notificationApplication,
            IHubContext<NotificationHub> hubContext
        )
        {
            _notificationApplication = notificationApplication;
            _hubContext = hubContext;
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

        [HttpPost("GetChannels")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<ChannelResponseDto>>>> GetChannels([FromBody] BaseFiltersRequest filters)
        {
            var response = await _notificationApplication.GetChannels(filters);

            return Ok(response);
        }

        [HttpPost("SendNotification")]
        public async Task<ActionResult<BaseResponse<bool>>> SendNotification([FromBody] NotificationRequestDto notificationRequestDto)
        {
            var response = await _notificationApplication.SendNotification(notificationRequestDto);

            if (response.IsSuccess)
            {
                await _hubContext.Clients.Group(notificationRequestDto.ChannelId.ToString())
                    .SendAsync("ReceiveMessage", notificationRequestDto.ChannelId, notificationRequestDto.Message);
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetNotificationsByChannel")]
        public async Task<ActionResult<BaseResponse<List<NotificationResponseDto>>>> GetNotificationsByChannel([FromQuery] long channelId)
        {
            var response = await _notificationApplication.GetNotificationsByChannel(channelId);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetChannelsByAthlete")]
        public async Task<ActionResult<BaseResponse<List<long>>>> GetChannelsByAthlete()
        {
            var response = await _notificationApplication.GetChannelsByAthlete();

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetNotificationsByAthlete")]
        public async Task<ActionResult<BaseResponse<List<NotificationResponseDto>>>> GetNotificationsByAthlete()
        {
            var response = await _notificationApplication.GetNotificationsByAthlete();

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
