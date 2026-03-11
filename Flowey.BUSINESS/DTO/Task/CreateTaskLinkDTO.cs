using Flowey.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class CreateTaskLinkDTO
    {
        public Guid TaskId { get; set; }
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }
    }
}
