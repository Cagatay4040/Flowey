using Flowey.API.Hubs;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Flowey.API.Services
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
