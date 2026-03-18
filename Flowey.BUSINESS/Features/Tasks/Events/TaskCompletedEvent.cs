using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Events
{
    public class TaskCompletedEvent : INotification
    {
        public Guid CompletedTaskId { get; set; }
        public Guid DoneStepId { get; set; }
        public Guid UserId { get; set; }

        public TaskCompletedEvent(Guid completedTaskId, Guid doneStepId, Guid userId)
        {
            CompletedTaskId = completedTaskId;
            DoneStepId = doneStepId;
            UserId = userId;
        }
    }
}
