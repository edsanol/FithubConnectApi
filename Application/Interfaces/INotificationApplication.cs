using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;

namespace Application.Interfaces
{
    public interface INotificationApplication
    {
        Task<BaseResponse<bool>> CreateChannel(ChannelRequestDto channelRequestDto);
        Task<BaseResponse<bool>> AddUserToChannel(UserChannelRequestDto userChannelRequestDto);
        Task<BaseResponse<bool>> RemoveUserFromChannel(UserChannelRequestDto userChannelRequestDto);
        Task<BaseResponse<List<ChannelResponseDto>>> GetChannels();
    }
}
