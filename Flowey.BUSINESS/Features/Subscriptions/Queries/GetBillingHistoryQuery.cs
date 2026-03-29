using AutoMapper;
using Flowey.CORE.DTO.User;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Subscriptions.Queries
{
    public class GetBillingHistoryQuery : IRequest<IDataResult<List<UserSubscriptionGetDTO>>>
    {
        public Guid UserId { get; set; }

        public GetBillingHistoryQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetBillingHistoryQueryHandler : IRequestHandler<GetBillingHistoryQuery, IDataResult<List<UserSubscriptionGetDTO>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityRepository<UserSubscription> _userSubscriptionRepository;
        private readonly IMapper _mapper;

        public GetBillingHistoryQueryHandler(IUserRepository userRepository, IEntityRepository<UserSubscription> userSubscriptionRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<UserSubscriptionGetDTO>>> Handle(GetBillingHistoryQuery request, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.AnyAsync(x => x.Id == request.UserId);

            if (!userExists)
                return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Error, Messages.UserNotFound, null);

            var entityList = await _userSubscriptionRepository.GetList(x => x.UserId == request.UserId, true, query => query.OrderByDescending(o => o.CreatedDate));

            if (entityList == null || !entityList.Any())
                return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Success, Messages.NoInvoicesFound, new List<UserSubscriptionGetDTO>());

            var data = _mapper.Map<List<UserSubscriptionGetDTO>>(entityList);

            return new DataResult<List<UserSubscriptionGetDTO>>(ResultStatus.Success, data);
        }
    }
}
