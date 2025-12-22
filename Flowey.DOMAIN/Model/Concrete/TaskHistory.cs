using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class TaskHistory : BaseEntity, IEntity
    {
        public Guid TaskId { get; set; }
        public Guid StepId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

        public virtual Task Task { get; set; }
        public virtual Step Step { get; set; }
        public virtual User User { get; set; }
    }
}
