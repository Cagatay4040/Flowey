using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Interfaces.Services;
using Flowey.INFRASTRUCTURE.Services.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Flowey.INFRASTRUCTURE.Services.Notifications
{
    public class SignalRNotificationManager : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationManager(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(UserNotificationAddDTO notification)
        {
            await _hubContext.Clients.User(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", notification.Title, notification.Message, notification.ActionUrl, notification.IsRead);
        }
    }
}
