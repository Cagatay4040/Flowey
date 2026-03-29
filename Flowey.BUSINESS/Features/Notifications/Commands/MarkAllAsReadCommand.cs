using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.Commands
{
    public class MarkAllAsReadCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }

        public MarkAllAsReadCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, IResult>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllAsReadCommandHandler(IUserNotificationRepository userNotificationRepository, IUnitOfWork unitOfWork)
        {
            _userNotificationRepository = userNotificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            var unreadNotifications = await _userNotificationRepository.GetList(x => x.UserId == request.UserId && !x.IsRead);

            if (unreadNotifications == null || !unreadNotifications.Any())
                return new Result(ResultStatus.Success, Messages.NoUnreadNotifications);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _userNotificationRepository.UpdateRangeAsync(unreadNotifications);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserNotificationUpdated);

            return new Result(ResultStatus.Error, Messages.UserNotificationUpdateError);
        }
    }
}
