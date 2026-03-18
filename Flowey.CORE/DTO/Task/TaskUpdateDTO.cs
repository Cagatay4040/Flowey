using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Task
{
    public class TaskUpdateDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
