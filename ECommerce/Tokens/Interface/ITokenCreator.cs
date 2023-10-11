using Microsoft.AspNetCore.Identity;

namespace ECommerce.Tokens.Interface
{
    public interface ITokenCreator
    {
        string CreateJwtToken(IdentityUser identityUser, List<string> roles);
    }
}
