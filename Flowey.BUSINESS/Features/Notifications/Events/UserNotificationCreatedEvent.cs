using MediatR;

namespace Flowey.BUSINESS.Features.Notifications.Events
{
    public class UserNotificationCreatedEvent : INotification
    {
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; } = false;

        public UserNotificationCreatedEvent(Guid userId, Guid senderId, string title, string message, string? actionUrl, bool ısRead)
        {
            UserId = userId;
            SenderId = senderId;
            Title = title;
            Message = message;
            ActionUrl = actionUrl;
            IsRead = ısRead;
        }
    }
}
