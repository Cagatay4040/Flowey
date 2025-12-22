using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Task : BaseEntity, IEntity
    {
        public string Title { get; set; }
        public string TaskKey { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
    }
}
