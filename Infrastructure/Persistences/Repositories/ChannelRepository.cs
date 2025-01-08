using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
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

        public async Task<List<long>> GetChannelsByAthleteId(int athleteId)
        {
            var response = await _context.ChannelUsers
                .Where(x => x.IdAthlete == athleteId)
                .Select(x => x.IdChannel)
                .ToListAsync();

            return response;
        }

        public async Task<BaseEntityResponse<Channels>> GetChannelsByGymId(BaseFiltersRequest filters, int gymId)
        {
            var response = new BaseEntityResponse<Channels>();

            var channels = _context.Channels
                .Include(x => x.ChannelUsers).ThenInclude(x => x.IdAthleteNavigation)
                .Include(x => x.Notifications)
                .Where(x => x.IdGym == gymId)
                .AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        channels = channels.Where(x => x.ChannelName.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        channels = channels.Where(x => x.ChannelUsers.Any(x => x.IdAthleteNavigation.AthleteId.Equals(Int32.Parse(filters.TextFilter))));
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                channels = channels.Where(x => x.CreatedAt >= DateTime.Parse(filters.StartDate));
            }

            if (!string.IsNullOrEmpty(filters.EndDate))
            {
                channels = channels.Where(x => x.CreatedAt <= DateTime.Parse(filters.EndDate));
            }

            filters.Sort ??= "ChannelId";
            response.TotalRecords = await channels.CountAsync();
            response.Items = await Ordering(filters, channels, !(bool)filters.Download!).ToListAsync();

            return response;
        }
    }
}
