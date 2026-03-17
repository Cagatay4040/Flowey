using Flowey.CORE.DTO.Notification;
using Flowey.CORE.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Interfaces.Services
{
    public interface IUserNotificationService
    {
        #region Get Methods

        Task<IDataResult<List<UserNotificationGetDTO>>> GetUserNotificationsAsync(Guid userId);

        #endregion

        #region Insert Methods

        Task<IResult> AddUserNotificationAsync(UserNotificationAddDTO dto);
        Task SendMentionNotificationsAsync(string content, Guid taskId, Guid projectId);

        #endregion

        #region Update Methods

        Task<IResult> MarkAsReadAsync(Guid notificationId);
        Task<IResult> MarkAllAsReadAsync(Guid userId);

        #endregion

        #region Delete Methods



        #endregion
    }
}
