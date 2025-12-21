using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class TaskAddDTO
    {
        public string Title { get; set; }
        public string TaskKey { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}
