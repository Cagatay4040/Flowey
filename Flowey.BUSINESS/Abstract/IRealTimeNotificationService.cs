using Flowey.BUSINESS.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(UserNotificationAddDTO notification);
    }
}
