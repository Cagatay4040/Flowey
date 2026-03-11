using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class RelatedTaskDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string RelationType { get; set; }
    }
}
