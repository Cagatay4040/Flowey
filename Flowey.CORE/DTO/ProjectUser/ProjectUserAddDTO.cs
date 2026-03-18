using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.ProjectUser
{
    public class ProjectUserAddDTO
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public RoleType RoleId { get; set; }
    }
}
