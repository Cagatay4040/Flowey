using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.Notification
{
    public class UserNotificationAddDTO
    {
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
