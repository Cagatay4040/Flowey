using Flowey.SHARED.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.Step
{
    public class StepUpdateDTO
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public StepCategory Category { get; set; }
    }
}
