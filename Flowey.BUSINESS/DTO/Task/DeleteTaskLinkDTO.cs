using System;

namespace Flowey.BUSINESS.DTO.Task
{
    public class DeleteTaskLinkDTO
    {
        public Guid SourceTaskId { get; set; }
        public Guid TargetTaskId { get; set; }
    }
}
