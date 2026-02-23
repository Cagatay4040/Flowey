using Flowey.BUSINESS.DTO.Notification;
using Flowey.CORE.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface IUserNotificationService
    {
        #region Get Methods

        Task<IDataResult<List<UserNotificationGetDTO>>> GetUserNotificationsAsync(Guid userId);

        #endregion

        #region Insert Methods

        Task<IResult> AddUserNotificationAsync(UserNotificationAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> MarkAsReadAsync(Guid notificationId);
        Task<IResult> MarkAllAsReadAsync(Guid userId);

        #endregion

        #region Delete Methods



        #endregion
    }
}
