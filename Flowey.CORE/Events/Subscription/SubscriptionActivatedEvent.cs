using MediatR;

namespace Flowey.CORE.Events.Subscription
{
    public class SubscriptionActivatedEvent : INotification
    {
        public Guid UserId { get; set; }

        public SubscriptionActivatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
