using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using ECommerce.Tokens;
using ECommerce.Tokens.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ECommerce.Controllers
{
    /// <summary>
    /// This API Controller handles all the Logic related to User Registration and Logins
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenCreator tokenCreator;
        private readonly IStringLocalizer<AuthController> localizer;
        private readonly ICustomerService customerRepositoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="tokenCreator"></param>
        /// <param name="localizer"></param>
        /// <param name="customerRepositoryService"></param>
        public AuthController(UserManager<IdentityUser> userManager, ITokenCreator tokenCreator, IStringLocalizer<AuthController> localizer, ICustomerService customerRepositoryService)
        {
            this.userManager = userManager;
            this.tokenCreator = tokenCreator;
            this.localizer = localizer;
            this.customerRepositoryService = customerRepositoryService;
        }

        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <param name="registerRequestDto">RegisterRequestDto Object</param>
        /// <returns>Returns 200OK response with a Message,CustomerId,EmailID in an object if creation is successful otherwise 400BadRequest with a Message,Error in a object</returns>
        [HttpPost("signup")]
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

        /// <summary>
        /// Verify if the user is valid
        /// </summary>
        /// <param name="loginRequestDto">LoginRequestDto object</param>
        /// <returns>Returns a 2000k response with jwtToken, CustomerId and CustomerEmail as an object otherwise a 400BadRequest</returns>
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
                            var jwtToken = tokenCreator.CreateJwtToken(user, roles.ToList());
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

        /// <summary>
        /// Checks if the User with given Email exists
        /// </summary>
        /// <param name="details">GetUserByEmailDto object</param>
        /// <returns>Returns 200Ok response if the User exists otherwise 404NotFound</returns>
        [HttpPost("find-user")]
        public async Task<IActionResult> GetUserByEmail([FromBody] GetUserByEmailDto details)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(details.Email);
                return user != null ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Resets the password of the user
        /// </summary>
        /// <param name="newDetails">LoginRequestDto object</param>
        /// <returns>Returns 200Ok response if password is reset successfully otherwise 400BadRequest both the responses will have an object with Message key</returns>
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] LoginRequestDto newDetails)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(newDetails.Email);
                var resultMessage = "";
                if (user == null)
                {
                    resultMessage = localizer["InvalidEmail"].Value;
                    return NotFound(new { Message = resultMessage });
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, newDetails.Password);
                resultMessage = result.Succeeded ? localizer["PasswordChangeSuccess"].Value : localizer["PasswordChangeFailure"].Value;
                return result.Succeeded ? Ok(new { Message = resultMessage }) : BadRequest(new { Message = resultMessage });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

    }
}
