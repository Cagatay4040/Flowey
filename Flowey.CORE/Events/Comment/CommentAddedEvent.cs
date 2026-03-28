using Flowey.CORE.Events.Common;

namespace Flowey.CORE.Events.Comment
{
    public class CommentAddedEvent : IHasMentionableContent
    {
        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string TaskKey { get; set; }
        public Guid ProjectId { get; set; }

        public CommentAddedEvent(string content, Guid taskId, Guid userId, string taskKey, Guid projectId)
        {
            Content = content;
            TaskId = taskId;
            UserId = userId;
            TaskKey = taskKey;
            ProjectId = projectId;
        }
    }
}
