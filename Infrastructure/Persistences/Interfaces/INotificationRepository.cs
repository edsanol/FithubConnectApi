using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> SaveNotification(Notifications notification);
        Task<List<Notifications>> GetNotificationsByChannel(long channelId);
        Task<List<Notifications>> GetNotificationsByAthlete(int athleteId);
    }
}
