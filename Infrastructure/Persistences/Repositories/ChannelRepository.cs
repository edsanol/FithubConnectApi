using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ChannelRepository : GenericRepository<Channels>, IChannelRepository
    {
        private readonly DbFithubContext _context;

        public ChannelRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateChannel(Channels channel)
        {
            await _context.Channels.AddAsync(channel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Channels> GetChannelById(long channelId)
        {
            var response = await _context.Channels
                .FirstOrDefaultAsync(x => x.ChannelId == channelId);

            return response ?? throw new Exception("Channel not found");
        }

        public async Task<List<Channels>> GetChannelsByGymId(int gymId)
        {
            var response = await _context.Channels
                .Include(x => x.ChannelUsers).ThenInclude(x => x.IdAthleteNavigation)
                .Include(x => x.Notifications)
                .Where(x => x.IdGym == gymId)
                .AsNoTracking()
                .ToListAsync();

            return response;
        }
    }
}
