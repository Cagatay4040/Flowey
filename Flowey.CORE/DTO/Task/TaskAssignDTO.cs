namespace Flowey.CORE.DTO.Task
{
    public class TaskAssignDTO
    {
        public Guid TaskId { get; set; }
        public Guid? UserId { get; set; }
    }
}
