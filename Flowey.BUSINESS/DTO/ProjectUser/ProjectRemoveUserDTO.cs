using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.ProjectUser
{
    public  class ProjectRemoveUserDTO
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
