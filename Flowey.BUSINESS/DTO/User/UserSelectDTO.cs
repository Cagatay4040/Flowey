using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.User
{
    public class UserSelectDTO
    {
        public Guid Id { get; set; }     
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
