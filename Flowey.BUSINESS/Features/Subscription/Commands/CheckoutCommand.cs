using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Subscription.Commands
{
    public class CheckoutCommand : IRequest<IDataResult<string>>
    {
        public int MonthsToPurchase { get; set; } = 1;

        public CheckoutCommand(int monthsToPurchase)
        {
            MonthsToPurchase = monthsToPurchase;
        }
    }

    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, IDataResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityRepository<UserSubscription> _userSubscriptionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(
            IUserRepository userRepository, 
            IEntityRepository<UserSubscription> userSubscriptionRepository, 
            ICurrentUserService currentUserService, 
            ITokenService tokenService, 
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IDataResult<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId().Value;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new DataResult<string>(ResultStatus.Error, Messages.UserNotFound, null);

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate;

            if (user.PremiumExpirationDate.HasValue && user.PremiumExpirationDate.Value > DateTime.UtcNow)
                endDate = user.PremiumExpirationDate.Value.AddMonths(request.MonthsToPurchase);
            else
                endDate = DateTime.UtcNow.AddMonths(request.MonthsToPurchase);

            var subscription = new UserSubscription
            {
                UserId = user.Id,
                PlanName = "Premium (1 Month)",
                IsPaid = true,
                StartDate = startDate,
                EndDate = endDate,
                Price = 299.90m * request.MonthsToPurchase,
                IsActive = true
            };

            user.PremiumExpirationDate = endDate;

            await _userSubscriptionRepository.AddAsync(subscription);
            await _userRepository.UpdateAsync(user);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow <= 0)
                return new DataResult<string>(ResultStatus.Error, Messages.SubscriptionFailed, null);

            var newToken = _tokenService.GenerateToken(user);

            return new DataResult<string>(ResultStatus.Success, Messages.PaymentSuccessful, newToken);
        }
    }
}
