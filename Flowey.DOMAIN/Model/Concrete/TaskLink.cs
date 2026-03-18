using Flowey.DOMAIN.Model.Abstract;
using Flowey.SHARED.Enums;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class TaskLink : BaseEntity, IEntity
    {
        public Guid SourceTaskId { get; set; }
        public Task SourceTask { get; set; }

        public Guid TargetTaskId { get; set; }
        public Task TargetTask { get; set; }

        public LinkType LinkType { get; set; }
    }
}
