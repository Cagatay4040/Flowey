using Flowey.CORE.DTO.Notification;

namespace Flowey.CORE.Interfaces.Services
{
    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(UserNotificationAddDTO notification);
    }
}
