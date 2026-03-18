using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class Project : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public string ProjectKey { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
        public virtual ICollection<ProjectUserRole> ProjectUserRoles { get; set; }
    }
}
