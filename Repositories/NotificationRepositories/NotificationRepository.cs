using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepositories;

namespace Repositories.NotificationRepositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly AXLMDbContext _context;

        public NotificationRepository(AXLMDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllNotifications()
        {
           return await _context.Notifications.OrderByDescending(n=> n.CreatedAt).ToListAsync();
        }
    }
}