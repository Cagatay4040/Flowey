using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.DOMAIN.Model.Abstract
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
}
