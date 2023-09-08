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
            try
            {
                var identityUser = new IdentityUser
                {
                    Email = registerRequestDto.Email,
                    UserName = registerRequestDto.Email
                };
                if (registerRequestDto.Roles.Any() && registerRequestDto.Roles != null && registerRequestDto.Roles.Length > 0)
                {
                    var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
                    if (identityResult.Succeeded)
                    {
                        identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                        if (identityResult.Succeeded)
                        {
                            var customerCredential = new CustomerCredential
                            {
                                CustomerId = Guid.Parse(identityUser.Id),
                                EmailId = identityUser.Email,
                                Password = identityUser.PasswordHash ?? ""
                            };
                            var createdCustomer = await customerRepositoryService.CreateCustomerAsync(customerCredential);
                            return Ok(new { Message = localizer["UserRegistrationSuccess"].Value, createdCustomer.CustomerId, createdCustomer.EmailId });
                        }
                        await userManager.DeleteAsync(identityUser);
                        return BadRequest(new { Message = localizer["RoleAdditionFailed"].Value, Error = identityResult.Errors.ToList() });
                    }
                    return BadRequest(new { Message = localizer["UserRegistrationFailure"].Value, Error = identityResult.Errors.ToList() });
                }
                return BadRequest(new { Message = localizer["EmptyRoles"].Value });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message , Details = ex.ToString()});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
                if (user != null)
                {
                    var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                    if (checkPasswordResult)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        if (roles != null)
                        {
                            var jwtToken = tokenProviderService.CreateJwtToken(user, roles.ToList());
                            return Ok(new { jwtToken });
                        }
                    }
                    return BadRequest(new { Message = localizer["InvalidPassword"].Value });
                }
                return BadRequest(new { Message = localizer["InvalidEmail"].Value });
            }
            catch(Exception ex)
            {
                return BadRequest(new {Error = ex.Message,  Details = ex.ToString()});
            }
        }


    }
}
