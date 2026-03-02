using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Role : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<ProjectUserRole> ProjectUserRoles { get; set; }
    }
}
