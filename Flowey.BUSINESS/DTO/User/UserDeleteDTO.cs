using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.User
{
    public class UserDeleteDTO
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }
    }
}
