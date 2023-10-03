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
        //This API will register a new user
        public async Task<IActionResult> Signup([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.Email
            };
            try
            {
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
                            var customerCart = new Cart
                            {
                                CartId = Guid.NewGuid(),
                                CustomerId = Guid.Parse(identityUser.Id)
                            };
                            var createdCustomer = await customerRepositoryService.CreateCustomerAsync(customerCredential);
                            await customerRepositoryService.CreateCartAsync(customerCart);
                            return Ok(new { Message = localizer["UserRegistrationSuccess"].Value, createdCustomer.CustomerId, createdCustomer.EmailId });
                        }
                        await userManager.DeleteAsync(identityUser);
                        return BadRequest(new { Message = localizer["RoleAdditionFailed"].Value, Error = identityResult.Errors.ToList() });
                    }
                    return BadRequest(new { Message = localizer["UserRegistrationFailure"].Value, Error = identityResult.Errors.ToList() });
                }
                return BadRequest(new { Message = localizer["EmptyRoles"].Value });
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(identityUser);
                return BadRequest(new { Error = ex.Message, Details = ex.ToString() });
            }
        }

        [HttpPost("login")]
        //This API will verify if the user is valid and will return a jwtToken
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
                            return Ok(new { jwtToken, CustomerId = user.Id, CustomerEmail = user.Email });
                        }
                    }
                    return BadRequest(new { Message = localizer["InvalidPassword"].Value });
                }
                return BadRequest(new { Message = localizer["InvalidEmail"].Value });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, Details = ex.ToString() });
            }
        }

        [HttpPost("find-user")]
        //This API will return 200Ok response if the user with the given mail id is found otherwise return 404NotFound
        public async Task<IActionResult> GetUserByEmail([FromBody] GetUserByEmailDto details)
        {
            var user = await userManager.FindByEmailAsync(details.Email);
            return user != null ? Ok() : NotFound();
        }

        [HttpPut("reset-password")]
        //This API will reset the password of the user in case if he forgets the password
        public async Task<IActionResult> ResetPassword([FromBody] LoginRequestDto newDetails)
        {
            var user = await userManager.FindByEmailAsync(newDetails.Email);
            var resultMessage = "";
            if (user == null)
            {
                resultMessage = localizer["InvalidEmail"].Value;
                return NotFound(resultMessage);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, newDetails.Password);
            resultMessage = result.Succeeded ? localizer["PasswordChangeSuccess"].Value : localizer["PasswordChangeFailure"].Value;
            return result.Succeeded ? Ok(new { Message = resultMessage }) : BadRequest(new { Message = resultMessage });
        }

    }
}
