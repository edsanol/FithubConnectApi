using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class NotificationRepository : GenericRepository<Notifications>, INotificationRepository
    {
        private readonly DbFithubContext _context;

        public NotificationRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> SaveNotification(Notifications notification)
        {
            await _context.Notifications.AddAsync(notification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
