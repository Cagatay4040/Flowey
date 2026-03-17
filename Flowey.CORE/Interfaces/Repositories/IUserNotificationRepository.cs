using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.CORE.Interfaces.Repositories
{
    public interface IUserNotificationRepository : IEntityRepository<UserNotification>
    {
        #region Get Methods

        Task<List<UserNotification>> GetUserNotifications(Guid userId);

        #endregion

        #region Insert Methods



        #endregion

        #region Update Methods



        #endregion

        #region Delete Methods



        #endregion
    }
}
