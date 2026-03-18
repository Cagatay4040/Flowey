using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class User : BaseEntity, IEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? PremiumExpirationDate { get; set; }
        public string? ProfileImageUrl { get; set; }

        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
        public virtual ICollection<ProjectUserRole> ProjectUserRoles { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }
    }
}
