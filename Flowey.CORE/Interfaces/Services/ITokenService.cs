using Flowey.DOMAIN.Model.Concrete;

namespace Flowey.CORE.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
