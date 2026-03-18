using Flowey.SHARED.Enums;

namespace Flowey.CORE.DTO.ProjectUser
{
    public class ProjectUserUpdateRoleDTO
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public RoleType RoleId { get; set; }
    }
}
