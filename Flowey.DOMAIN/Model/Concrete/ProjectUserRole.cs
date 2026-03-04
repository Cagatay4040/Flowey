using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.CORE.Enums;
using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class ProjectUserRole : BaseEntity, IEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public RoleType RoleId { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
