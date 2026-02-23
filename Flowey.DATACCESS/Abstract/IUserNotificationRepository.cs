using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DATACCESS.Abstract
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
