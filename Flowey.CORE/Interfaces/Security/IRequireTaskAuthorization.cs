using Flowey.SHARED.Enums;

namespace Flowey.CORE.Interfaces.Security
{
    public interface IRequireTaskAuthorization
    {
        Guid TaskId { get; }
        RoleType[] RequiredRoles { get; }
    }
}
