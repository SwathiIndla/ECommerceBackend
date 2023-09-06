using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenProviderService;
        private readonly IStringLocalizer<AuthController> localizer;
        private readonly ICustomerRepository customerRepositoryService;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenProviderService, IStringLocalizer<AuthController> localizer, ICustomerRepository customerRepositoryService)
        {
            this.userManager = userManager;
            this.tokenProviderService = tokenProviderService;
            this.localizer = localizer;
            this.customerRepositoryService = customerRepositoryService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.Email
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if(registerRequestDto.Roles.Any() && registerRequestDto.Roles != null)
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser,registerRequestDto.Roles);
                }
                if (identityResult.Succeeded)
                {
                    var customerCredential = new CustomerCredential
                    {
                        EmailId = identityUser.Email,
                        Password = identityUser.PasswordHash ?? ""
                    };
                    var createdCustomer = await customerRepositoryService.CreateCustomerAsync(customerCredential);
                    return Ok(new { Message = localizer["UserRegistrationSuccess"].Value, createdCustomer.CustomerId, createdCustomer.EmailId });
                }
                return BadRequest(new { Message = localizer["SomethingWentWrong"].Value, Error = identityResult.Errors.ToList() });
            }
            return BadRequest(new { Message = localizer["SomethingWentWrong"].Value, Error = identityResult.Errors.ToList() });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
            if(user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if(roles != null)
                    {
                        var jwtToken = tokenProviderService.CreateJwtToken(user, roles.ToList());
                        return Ok(new { jwtToken });
                    }
                }
                return BadRequest(new {Message = localizer["InvalidPassword"].Value });
            }
            return BadRequest(new {Message = localizer["InvalidEmail"].Value });
        }
    }
}
