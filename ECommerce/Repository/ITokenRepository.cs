using Microsoft.AspNetCore.Identity;

namespace ECommerce.Repository
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser identityUser, List<string> roles);
    }
}
