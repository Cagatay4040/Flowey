using Flowey.BUSINESS.Features.Notifications.Commands;
using Flowey.BUSINESS.Features.Tasks.Events;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.EventHandlers
{
    public class SendNotificationOnTaskAssigneeChangedEventHandler : INotificationHandler<TaskAssigneeChangedEvent>
    {
        private readonly IMediator _sender;
        private readonly IUserRepository _userRepository;

        public SendNotificationOnTaskAssigneeChangedEventHandler(IMediator sender, IUserRepository userRepository)
        {
            _sender = sender;
            _userRepository = userRepository;
        }

        public async Task Handle(TaskAssigneeChangedEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.NewAssigneeId.HasValue || notification.NewAssigneeId.Value == notification.AssignerId)
                return;

            string taskIdentifier = notification.TaskKey != null
                                    ? $"task #{notification.TaskKey}"
                                    : "a task";

            var senderUser = await _userRepository.GetByIdAsync(notification.AssignerId);
            string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

            await _sender.Send(new CreateUserNotificationCommand(
                notification.NewAssigneeId.Value,
                notification.AssignerId,
                Messages.TaskReassignedTitle,
                string.Format(Messages.TaskReassignedMessage, senderName, taskIdentifier),
                $"/board/{notification.ProjectId}?taskId={notification.TaskId}",
                false
            ), cancellationToken);
        }
    }
}
