using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class UserNotification : BaseEntity, IEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Message { get; set; } 
        public string? ActionUrl { get; set; } 
        public bool IsRead { get; set; } = false;

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
