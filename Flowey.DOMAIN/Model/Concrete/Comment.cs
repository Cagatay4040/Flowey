using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Comment : BaseEntity, IEntity
    {
        public Comment()
        {
            Attachments = new HashSet<CommentAttachment>();
        }

        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<CommentAttachment> Attachments { get; set; }
    }
}
