namespace Flowey.CORE.DataAccess.Abstract
{
    public interface ICurrentUserService
    {
        Guid? GetUserId();
        Guid? GetUserIdOrDefault();
    }
}
