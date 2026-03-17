using Flowey.DOMAIN.Model.Abstract;
using Flowey.SHARED.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
