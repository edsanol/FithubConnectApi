using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class NotificationRepository : GenericRepository<Notifications>, INotificationRepository
    {
        private readonly DbFithubContext _context;

        public NotificationRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Notifications>> GetNotificationsByAthlete(int athleteId)
        {
            return await _context.Notifications
                .Where(n => n.IdChannelNavigation.ChannelUsers.Any(cu => cu.IdAthlete == athleteId) &&
                    n.SendAt >= DateTime.Now.AddDays(-7))
                .OrderByDescending(n => n.SendAt)
                .ToListAsync();
        }

        public async Task<List<Notifications>> GetNotificationsByChannel(long channelId)
        {
            return await _context.Notifications
                .Where(n => n.IdChannel == channelId)
                .OrderBy(n => n.SendAt)
                .ToListAsync();
        }

        public async Task<bool> SaveNotification(Notifications notification)
        {
            await _context.Notifications.AddAsync(notification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
