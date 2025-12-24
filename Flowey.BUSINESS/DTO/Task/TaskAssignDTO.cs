using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class TaskAssignDTO
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
    }
}
