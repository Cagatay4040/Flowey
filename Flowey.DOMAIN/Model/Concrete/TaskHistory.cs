using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class TaskHistory : BaseEntity, IEntity
    {
        public Guid TaskId { get; set; }
        public Guid StepId { get; set; }
        public Guid? UserId { get; set; }

        public virtual Task Task { get; set; }
        public virtual Step Step { get; set; }
        public virtual User? User { get; set; }
    }
}
