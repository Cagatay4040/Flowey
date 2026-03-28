using MediatR;

namespace Flowey.CORE.Events.Task
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
