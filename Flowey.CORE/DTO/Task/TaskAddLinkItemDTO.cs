using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.Task
{
    public class TaskAddLinkItemDTO
    {
        public Guid TargetTaskId { get; set; }
        public LinkType LinkType { get; set; }
    }
}
