using AutoMapper;
using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.Queries
{
    public class GetUserNotificationsQuery : IRequest<IDataResult<List<UserNotificationGetDTO>>>
    {
        public Guid UserId { get; set; }

        public GetUserNotificationsQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IDataResult<List<UserNotificationGetDTO>>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserNotificationsQueryHandler(IUserNotificationRepository userNotificationRepository, IUserRepository userRepository, IMapper mapper)
        {
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<UserNotificationGetDTO>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return new DataResult<List<UserNotificationGetDTO>>(ResultStatus.Error, Messages.UserNotFound, new List<UserNotificationGetDTO>());

            var entityList = await _userNotificationRepository.GetUserNotifications(request.UserId);
            var data = _mapper.Map<List<UserNotificationGetDTO>>(entityList);
            return new DataResult<List<UserNotificationGetDTO>>(ResultStatus.Success, data);
        }
    }
}
