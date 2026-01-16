using System;

namespace Flowey.BUSINESS.DTO.Comment
{
    public class CommentUpdateDTO
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
    }
}
