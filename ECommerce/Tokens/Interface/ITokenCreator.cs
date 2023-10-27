using Microsoft.AspNetCore.Identity;

namespace ECommerce.Tokens.Interface
{
    public interface ITokenCreator
    {
        string CreateJwtToken(string email, List<string> roles);
    }
}
