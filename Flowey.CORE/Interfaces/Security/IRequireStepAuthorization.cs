using Flowey.SHARED.Enums;

namespace Flowey.CORE.Interfaces.Security
{
    public interface IRequireStepAuthorization
    {
        Guid StepId { get; }
        RoleType[] RequiredRoles { get; }
    }
}
