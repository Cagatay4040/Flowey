using Flowey.DOMAIN.Model.Abstract;
using Flowey.SHARED.Enums;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class ProjectUserRole : BaseEntity, IEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public RoleType RoleId { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
