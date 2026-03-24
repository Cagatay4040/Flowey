using Flowey.SHARED.Enums;

namespace Flowey.CORE.Interfaces.Security
{
    public interface IRequireProjectAuthorization
    {
        Guid ProjectId { get; }
        RoleType[] RequiredRoles { get; }
    }
}
