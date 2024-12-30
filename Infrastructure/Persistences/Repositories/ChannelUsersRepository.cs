using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class ChannelUsersRepository : GenericRepository<ChannelUsers>, IChannelUsersRepository
    {
        private readonly DbFithubContext _context;

        public ChannelUsersRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddUsersToChannel(List<ChannelUsers> channel)
        {
            await _context.ChannelUsers.AddRangeAsync(channel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUsersFromChannel(List<ChannelUsers> channel)
        {
            _context.ChannelUsers.RemoveRange(channel);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
