using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IChannelUsersRepository
    {
        Task<bool> AddUsersToChannel(List<ChannelUsers> channel, long channelId);
        Task<bool> RemoveUsersFromChannel(List<ChannelUsers> channel);
        Task<List<int>> GetAllAthleteIdsByChannel(long channelId);
    }
}
