using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Constants;
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
        private readonly IMapper _mapper;

        public SubscriptionManager(IUserRepository userRepository, IAuthService authService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _userRepository = userRepository;
            _authService = authService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<IDataResult<List<UserSubscriptionGetDTO>>> GetBillingHistoryAsync(Guid userId)
        {
            var userExists = await _userRepository.AnyAsync(x => x.Id == userId);

            if (!userExists)
                return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Error, Messages.UserNotFound, null);

            var entityList = await _userRepository.GetBillingHistoryAsync(userId);

            if (entityList == null || !entityList.Any())
                return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Success, Messages.NoInvoicesFound, new List<UserSubscriptionGetDTO>());

            var data = _mapper.Map<List<UserSubscriptionGetDTO>>(entityList);

            return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Success, data);
        }

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
                PlanName = "Premium (1 Month)",
                IsPaid = true,
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
