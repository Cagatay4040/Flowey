using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.Commands
{
    public class MarkAsReadCommand : IRequest<IResult>
    {
        public Guid NotificationId  { get; set; }

        public MarkAsReadCommand(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }

    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, IResult>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAsReadCommandHandler(IUserNotificationRepository userNotificationRepository, IUnitOfWork unitOfWork)
        {
            _userNotificationRepository = userNotificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _userNotificationRepository.GetByIdAsync(request.NotificationId);

            if (notification == null)
                return new Result(ResultStatus.Error, Messages.UserNotificationNotFound);

            notification.IsRead = true;

            await _userNotificationRepository.UpdateAsync(notification);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.UserNotificationUpdated);

            return new Result(ResultStatus.Error, Messages.UserNotificationUpdateError);
        }
    }
}
