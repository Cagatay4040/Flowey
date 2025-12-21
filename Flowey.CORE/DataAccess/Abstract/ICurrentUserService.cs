using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.DataAccess.Abstract
{
    public interface ICurrentUserService
    {
        Guid? GetUserId();
    }
}
