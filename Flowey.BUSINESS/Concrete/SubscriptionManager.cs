using Flowey.BUSINESS.Abstract;
using Flowey.CORE.Constants;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Concrete
{
    public class SubscriptionManager : ISubscriptionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public SubscriptionManager(IUserRepository userRepository, IAuthService authService, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _authService = authService;
            _currentUserService = currentUserService;
        }

        #region Get Methods



        #endregion

        #region Add Methods

        public async Task<IDataResult<string>> CheckoutAsync(UserCheckoutRequestDTO checkoutDto)
        {
            var userId = _currentUserService.GetUserId().Value;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new DataResult<string>(ResultStatus.Error, Messages.UserNotFound, null);

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate;

            if (user.PremiumExpirationDate.HasValue && user.PremiumExpirationDate.Value > DateTime.UtcNow)
                endDate = user.PremiumExpirationDate.Value.AddMonths(checkoutDto.MonthsToPurchase);
            else
                endDate = DateTime.UtcNow.AddMonths(checkoutDto.MonthsToPurchase);

            var subscription = new UserSubscription
            {
                UserId = user.Id,
                StartDate = startDate,
                EndDate = endDate,
                Price = 299.90m * checkoutDto.MonthsToPurchase,
                IsActive = true
            };

            user.PremiumExpirationDate = endDate;

            int effectedRow = await _userRepository.SubscribeUserAsync(user, subscription);

            if (effectedRow <= 0)
                return new DataResult<string>(ResultStatus.Error, Messages.SubscriptionFailed, null);

            var newToken = _authService.GetToken(user);

            return new DataResult<string>(ResultStatus.Success, Messages.PaymentSuccessful, newToken);
        }

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
