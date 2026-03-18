using Flowey.BUSINESS.Features.Common;
using Flowey.BUSINESS.Features.Notifications.Commands;
using Flowey.CORE.Interfaces.Repositories;
using Flowey.SHARED.Constants;
using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.EventHandlers
{
    public class SendMentionNotificationEventHandler : INotificationHandler<IHasMentionableContent>
    {
        private readonly IMediator _sender;
        private readonly IUserRepository _userRepository;

        public SendMentionNotificationEventHandler(IMediator sender, IUserRepository userRepository)
        {
            _sender = sender;
            _userRepository = userRepository;
        }

        public async Task Handle(IHasMentionableContent notification, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(notification.Content)) return;

            var mentionedUserIds = ExtractMentionedUserIds(notification.Content);
            if (!mentionedUserIds.Any()) return;

            var senderUser = await _userRepository.GetByIdAsync(notification.UserId);
            string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

            string taskIdentifier = notification.TaskKey != null ? $"task #{notification.TaskKey}" : "a task";

            foreach (var mentionedUserId in mentionedUserIds)
            {
                if (mentionedUserId != notification.UserId)
                {
                    await _sender.Send(new CreateUserNotificationCommand(
                        mentionedUserId,
                        notification.UserId,
                        Messages.NewMentionTitle,
                        string.Format(Messages.NewMentionMessage, senderName, taskIdentifier),
                        $"/board/{notification.ProjectId}?taskId={notification.TaskId}",
                        false
                    ), cancellationToken);
                }
            }
        }

        private List<Guid> ExtractMentionedUserIds(string htmlContent)
        {
            var userIds = new List<Guid>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return userIds;

            var regex = new System.Text.RegularExpressions.Regex(@"data-id=""([a-fA-F0-9\-]{36})""");
            var matches = regex.Matches(htmlContent);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count > 1 && Guid.TryParse(match.Groups[1].Value, out Guid parsedId))
                {
                    if (!userIds.Contains(parsedId))
                    {
                        userIds.Add(parsedId);
                    }
                }
            }
            return userIds;
        }
    }
}
