using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DTO.ProjectUser
{
    public class TransferOwnershipDTO
    {
        public Guid ProjectId { get; set; }
        public Guid NewOwnerId { get; set; }
    }
}
