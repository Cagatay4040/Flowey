using Flowey.CORE.DTO.Task;
using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Step
{
    public class StepGetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public StepCategory Category { get; set; }
        public List<TaskGetDTO> Tasks { get; set; }
    }
}
