namespace Flowey.CORE.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid? GetUserId();
        Guid? GetUserIdOrDefault();
    }
}
