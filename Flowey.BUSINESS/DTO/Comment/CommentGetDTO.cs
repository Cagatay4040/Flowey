using System;
using System.Collections.Generic;

namespace Flowey.BUSINESS.DTO.Comment
{
    public class CommentGetDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
