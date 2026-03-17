using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.Project
{
    public class ProjectGetDTO
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string ProjectKey { get; set; }
        public string CurrentUserRole { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
