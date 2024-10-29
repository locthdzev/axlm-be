using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.NotificationRepositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetAllNotifications();
    }
}