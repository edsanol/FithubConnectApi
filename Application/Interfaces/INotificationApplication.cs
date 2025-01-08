using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface INotificationApplication
    {
        Task<BaseResponse<bool>> CreateChannel(ChannelRequestDto channelRequestDto);
        Task<BaseResponse<bool>> AddUserToChannel(UserChannelRequestDto userChannelRequestDto);
        Task<BaseResponse<bool>> RemoveUserFromChannel(UserChannelRequestDto userChannelRequestDto);
        Task<BaseResponse<BaseEntityResponse<ChannelResponseDto>>> GetChannels(BaseFiltersRequest filters);
        Task<BaseResponse<bool>> SendNotification(NotificationRequestDto notificationRequestDto);
        Task<BaseResponse<List<NotificationResponseDto>>> GetNotificationsByChannel(long channelId);
        Task<BaseResponse<List<long>>> GetChannelsByAthlete();
        Task<BaseResponse<List<NotificationResponseDto>>> GetNotificationsByAthlete();
    }
}
