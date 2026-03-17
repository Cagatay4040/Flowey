using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
