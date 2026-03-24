using Flowey.SHARED.Enums;

namespace Flowey.CORE.Interfaces.Security
{
    public interface IRequireCommentAuthorization
    {
        Guid CommentId { get; }
        RoleType[] RequiredRoles { get; }
    }
}
