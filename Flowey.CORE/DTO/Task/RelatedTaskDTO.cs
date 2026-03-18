namespace Flowey.CORE.DTO.Task
{
    public class RelatedTaskDTO
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string TaskKey { get; set; }
        public string RelationType { get; set; }
    }
}
