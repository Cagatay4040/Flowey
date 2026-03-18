using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Step
{
    public class StepAddDTO
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public StepCategory Category { get; set; }
        public Guid ProjectId { get; set; }
    }
}
