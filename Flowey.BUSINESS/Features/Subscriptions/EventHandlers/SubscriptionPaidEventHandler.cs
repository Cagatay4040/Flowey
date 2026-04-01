using Flowey.CORE.Events.Subscription;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace Flowey.BUSINESS.Features.Subscriptions.EventHandlers
{
    public class SubscriptionPaidEventHandler : INotificationHandler<SubscriptionPaidEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityRepository<UserSubscription> _userSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public SubscriptionPaidEventHandler(
            IUserRepository userRepository, 
            IEntityRepository<UserSubscription> userSubscriptionRepository, 
            IUnitOfWork unitOfWork, 
            IPublisher publisher)
        {
            _userRepository = userRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task Handle(SubscriptionPaidEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(notification.UserId);
            if (user == null) return;

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = (user.PremiumExpirationDate.HasValue && user.PremiumExpirationDate.Value > DateTime.UtcNow)
                ? user.PremiumExpirationDate.Value.AddMonths(notification.MonthsToPurchase)
                : DateTime.UtcNow.AddMonths(notification.MonthsToPurchase);

            var subscription = new UserSubscription
            {
                UserId = user.Id,
                PlanName = $"Premium ({notification.MonthsToPurchase} Month)",
                IsPaid = true,
                StartDate = startDate,
                EndDate = endDate,
                Price = 299.90m * notification.MonthsToPurchase,
                IsActive = true
            };

            user.PremiumExpirationDate = endDate;

            await _userSubscriptionRepository.AddAsync(subscription);
            await _userRepository.UpdateAsync(user);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow == 0)
                throw new Exception(Messages.WebhookDatabaseCommitFailed);

            await _publisher.Publish(new SubscriptionActivatedEvent(user.Id), cancellationToken);
        }
    }
}
