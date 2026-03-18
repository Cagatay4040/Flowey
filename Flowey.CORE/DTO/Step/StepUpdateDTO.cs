using Flowey.SHARED.Enums;

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
