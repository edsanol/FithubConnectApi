using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ChannelUsersRepository : GenericRepository<ChannelUsers>, IChannelUsersRepository
    {
        private readonly DbFithubContext _context;

        public ChannelUsersRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddUsersToChannel(List<ChannelUsers> channel, long channelId)
        {
            _context.ChannelUsers.RemoveRange(_context.ChannelUsers.Where(x => x.IdChannel == channelId));
            await _context.ChannelUsers.AddRangeAsync(channel);

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<List<int>> GetAllAthleteIdsByChannel(long channelId)
        {
            return await _context.ChannelUsers
                .Where(x => x.IdChannel == channelId)
                .Select(x => x.IdAthlete)
                .ToListAsync();
        }

        public async Task<bool> RemoveUsersFromChannel(List<ChannelUsers> channel)
        {
            _context.ChannelUsers.RemoveRange(channel);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
