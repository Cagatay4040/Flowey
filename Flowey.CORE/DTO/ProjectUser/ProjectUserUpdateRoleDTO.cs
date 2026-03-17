using Flowey.SHARED.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.ProjectUser
{
    public class ProjectUserUpdateRoleDTO
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public RoleType RoleId { get; set; }
    }
}
