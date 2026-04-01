using Flowey.CORE.Events.Subscription;
using Flowey.CORE.Interfaces.Services;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.EventHandlers
{
    public class NotifyUserOnSubscriptionActivatedEventHandler : INotificationHandler<SubscriptionActivatedEvent>
    {
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public NotifyUserOnSubscriptionActivatedEventHandler(IRealTimeNotificationService realTimeNotificationService)
        {
            _realTimeNotificationService = realTimeNotificationService;
        }

        public async Task Handle(SubscriptionActivatedEvent notification, CancellationToken cancellationToken)
        {
            await _realTimeNotificationService.SendPaymentSuccessAsync(notification.UserId);
        }
    }
}
