using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class User : BaseEntity, IEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
        public virtual ICollection<ProjectUserRole> ProjectUserRoles { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
