using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.DOMAIN.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Step : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public StepCategory Category { get; set; }
        public Guid ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
