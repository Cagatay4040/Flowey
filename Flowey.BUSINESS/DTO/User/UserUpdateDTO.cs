using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.User
{
    public class UserUpdateDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }
    }
}
