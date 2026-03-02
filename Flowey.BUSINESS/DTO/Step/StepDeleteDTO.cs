using System;

namespace Flowey.BUSINESS.DTO.Step
{
    public class StepDeleteDTO
    {
        public Guid StepId { get; set; }
        public Guid? TargetStepId { get; set; }
    }
}
