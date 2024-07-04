using DB_AngoraLib.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationService
    {
        private readonly IGRepository<Notification> _dbRepository;

        public NotificationService(IGRepository<Notification> dbRepository)
        {
            _dbRepository = dbRepository;
        }

        public async Task CreateNotificationAsync(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await _dbRepository.AddObjectAsync(notification);
        }

        public async Task<List<Notification>> GetUnreadNotificationsForUserAsync(string userId)
        {
            return await _dbRepository.GetDbSet()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _dbRepository.GetObject_ByIntKEYAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _dbRepository.UpdateObjectAsync(notification);
            }
        }
    }
}
