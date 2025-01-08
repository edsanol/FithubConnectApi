using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IChannelRepository
    {
        Task<bool> CreateChannel(Channels channel);
        Task<BaseEntityResponse<Channels>> GetChannelsByGymId(BaseFiltersRequest filters, int gymId);
        Task<Channels> GetChannelById(long channelId);
        Task<List<long>> GetChannelsByAthleteId(int athleteId);
    }
}
