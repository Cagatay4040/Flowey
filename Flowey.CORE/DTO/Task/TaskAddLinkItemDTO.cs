using Flowey.SHARED.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.Task
{
    public class TaskAddLinkItemDTO
    {
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }
    }
}
