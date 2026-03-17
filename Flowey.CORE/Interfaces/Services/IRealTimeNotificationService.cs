using Flowey.CORE.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Interfaces.Services
{
    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(UserNotificationAddDTO notification);
    }
}
