using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Step
{
    public class StepAddDTO
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
    }
}
