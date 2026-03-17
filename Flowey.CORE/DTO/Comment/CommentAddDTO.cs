using System;

namespace Flowey.CORE.DTO.Comment
{
    public class CommentAddDTO
    {
        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
    }
}
