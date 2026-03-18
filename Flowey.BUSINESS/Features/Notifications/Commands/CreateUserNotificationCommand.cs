using Flowey.BUSINESS.Features.Notifications.Events;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.CORE.Interfaces.UnitOfWork;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DOMAIN.Model.Concrete;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.Commands
{
    public class CreateUserNotificationCommand : IRequest<IResult>
    {
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; } = false;

        public CreateUserNotificationCommand(Guid userId, Guid senderId, string title, string message, string? actionUrl, bool isRead)
        {
            UserId = userId;
            SenderId = senderId;
            Title = title;
            Message = message;
            ActionUrl = actionUrl;
            IsRead = isRead;
        }
    }

    public class CreateUserNotificationCommandHandler : IRequestHandler<CreateUserNotificationCommand, IResult>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserNotificationCommandHandler(
            IUserNotificationRepository userNotificationRepository, 
            IUserRepository userRepository, 
            IPublisher publisher,
            IUnitOfWork unitOfWork)
        {
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
            _publisher = publisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(CreateUserNotificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return new Result(ResultStatus.Error, Messages.UserNotFound);

            var userNotification = new UserNotification
            {
                UserId = request.UserId,
                SenderId= request.SenderId,
                Title = request.Title,
                Message = request.Message,
                ActionUrl = request.ActionUrl,
                IsRead = request.IsRead
            };

            await _userNotificationRepository.AddAsync(userNotification);
            int effectedRow = await _unitOfWork.SaveChangesAsync();

            if (effectedRow > 0)
            {
                var notificationEvent = new UserNotificationCreatedEvent(request.UserId, request.SenderId, request.Title, request.Message, request.ActionUrl, request.IsRead);
                await _publisher.Publish(notificationEvent, cancellationToken);
                return new Result(ResultStatus.Success, Messages.UserNotificationAdded);
            }

            return new Result(ResultStatus.Error, Messages.UserNotificationCreateError);
        }
    }
}
