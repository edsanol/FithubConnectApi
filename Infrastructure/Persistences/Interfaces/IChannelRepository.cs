using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IChannelRepository
    {
        Task<bool> CreateChannel(Channels channel);
        Task<List<Channels>> GetChannelsByGymId(int gymId);
    }
}
