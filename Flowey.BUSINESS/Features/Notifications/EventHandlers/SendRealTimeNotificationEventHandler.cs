using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Events.Notification;
using Flowey.CORE.Interfaces.Services;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.EventHandlers
{
    public class SendRealTimeNotificationEventHandler : INotificationHandler<UserNotificationCreatedEvent>
    {
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public SendRealTimeNotificationEventHandler(IRealTimeNotificationService realTimeNotificationService)
        {
            _realTimeNotificationService = realTimeNotificationService;
        }

        async System.Threading.Tasks.Task INotificationHandler<UserNotificationCreatedEvent>.Handle(UserNotificationCreatedEvent notification, CancellationToken cancellationToken)
        {
            var userNotification = new UserNotificationAddDTO
            {
                UserId = notification.UserId,
                SenderId = notification.SenderId,
                Title = notification.Title,
                Message = notification.Message,
                ActionUrl = notification.ActionUrl,
                IsRead = notification.IsRead
            };

            await _realTimeNotificationService.SendNotificationAsync(userNotification);
        }
    }
}
