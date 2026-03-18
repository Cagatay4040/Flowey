using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class UserNotification : BaseEntity, IEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid SenderId { get; set; }
        public User SenderUser { get; set; }

        public string Title { get; set; }
        public string Message { get; set; } 
        public string? ActionUrl { get; set; } 
        public bool IsRead { get; set; } = false;
    }
}
