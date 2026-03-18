using MediatR;

namespace Flowey.BUSINESS.Features.Common
{
    public interface IHasMentionableContent : INotification
    {
        string Content { get; }
        Guid UserId { get; }
        Guid TaskId { get; }
        Guid ProjectId { get; }
        string? TaskKey { get; }
    }
}
