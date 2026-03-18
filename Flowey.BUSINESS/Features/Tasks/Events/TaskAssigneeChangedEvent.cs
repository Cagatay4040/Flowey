using MediatR;

namespace Flowey.BUSINESS.Features.Tasks.Events
{
    public class TaskAssigneeChangedEvent : INotification
    {
        public Guid TaskId { get; set; }
        public Guid? NewAssigneeId { get; set; }
        public Guid AssignerId { get; set; }
        public Guid ProjectId { get; set; }
        public string? TaskKey { get; set; }

        public TaskAssigneeChangedEvent(Guid taskId, Guid? newAssigneeId, Guid assignerId, Guid projectId, string? taskKey)
        {
            TaskId = taskId;
            NewAssigneeId = newAssigneeId;
            AssignerId = assignerId;
            ProjectId = projectId;
            TaskKey = taskKey;
        }
    }
}
