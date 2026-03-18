using Flowey.DOMAIN.Model.Abstract;
using Flowey.SHARED.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Task : BaseEntity, IEntity
    {
        public Task()
        {
            TaskHistories = new List<TaskHistory>();
        }

        public string Title { get; set; }
        public string TaskKey { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; } = PriorityType.Medium;
        public DateTime? Deadline { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CurrentStepId { get; set; }
        public Guid? AssigneeId { get; set; }

        public virtual Project Project { get; set; }

        [ForeignKey("CurrentStepId")]
        public virtual Step Step { get; set; }

        [ForeignKey("AssigneeId")]
        public virtual User? User { get; set; }
        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
        public ICollection<TaskLink> OutgoingLinks { get; set; } = new List<TaskLink>();
        public ICollection<TaskLink> IncomingLinks { get; set; } = new List<TaskLink>();
    }
}
