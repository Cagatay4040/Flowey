using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Task
{
    public class TaskAddDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public List<TaskAddLinkItemDTO> Links { get; set; } = new List<TaskAddLinkItemDTO>();
    }
}
