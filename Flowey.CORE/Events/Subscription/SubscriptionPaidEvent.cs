using MediatR;

namespace Flowey.CORE.Events.Subscription
{
    public class SubscriptionPaidEvent : INotification
    {
        public Guid UserId { get; set; }
        public int MonthsToPurchase { get; set; }
    }
}
