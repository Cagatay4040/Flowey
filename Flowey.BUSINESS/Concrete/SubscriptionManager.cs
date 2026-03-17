using AutoMapper;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.DTO.User;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;

namespace Flowey.BUSINESS.Concrete
{
    public class SubscriptionManager : ISubscriptionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityRepository<UserSubscription> _userSubscriptionRepository;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionManager(IUserRepository userRepository, IEntityRepository<UserSubscription> userSubscriptionRepository, ITokenService tokenService, ICurrentUserService currentUserService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        public async Task<IDataResult<List<UserSubscriptionGetDTO>>> GetBillingHistoryAsync(Guid userId)
        {
            var userExists = await _userRepository.AnyAsync(x => x.Id == userId);

            if (!userExists)
                return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Error, Messages.UserNotFound, null);

            var entityList = await _userSubscriptionRepository.GetList(x => x.UserId == userId, true, query => query.OrderByDescending(o => o.CreatedDate));

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

            await _userSubscriptionRepository.AddAsync(subscription);
            await _userRepository.UpdateAsync(user);

            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow <= 0)
                return new DataResult<string>(ResultStatus.Error, Messages.SubscriptionFailed, null);

            var newToken = _tokenService.GenerateToken(user);

            return new DataResult<string>(ResultStatus.Success, Messages.PaymentSuccessful, newToken);
        }

        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
