using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Task
{
    public class CreateTaskLinkDTO
    {
        public Guid TaskId { get; set; }
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }
    }
}
