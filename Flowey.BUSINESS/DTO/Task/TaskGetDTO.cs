using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class TaskGetDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TaskKey { get; set; }
        public string Description { get; set; }
    }
}
