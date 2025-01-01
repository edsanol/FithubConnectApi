using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> SaveNotification(Notifications notification);
    }
}
