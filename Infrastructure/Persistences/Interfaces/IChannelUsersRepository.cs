using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IChannelUsersRepository
    {
        Task<bool> AddUsersToChannel(List<ChannelUsers> channel);
        Task<bool> RemoveUsersFromChannel(List<ChannelUsers> channel);
    }
}
