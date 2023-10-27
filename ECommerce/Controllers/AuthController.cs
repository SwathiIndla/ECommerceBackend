using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using ECommerce.Tokens;
using ECommerce.Tokens.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.Json;

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
        /// <response code="200">Returns a Message, CustomerId, EmailId of the newly created user</response>
        /// <response code="400">Returns a Bad request with a Message, when the user creation fails</response>
        /// <response code="500">Returns Internal server error with Error and Details when an exception occurs</response>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] RegisterRequestDto registerRequestDto)
        {
            //var identityUser = new IdentityUser
            //{
            //    Email = registerRequestDto.Email,
            //    UserName = registerRequestDto.Email
            //};
            //try
            //{
            //    if (registerRequestDto.Roles.Any() && registerRequestDto.Roles != null && registerRequestDto.Roles.Length > 0)
            //    {
            //        var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            //        if (identityResult.Succeeded)
            //        {
            //            identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
            //            if (identityResult.Succeeded)
            //            {
            //                var customerCredential = new CustomerCredential
            //                {
            //                    CustomerId = Guid.Parse(identityUser.Id),
            //                    EmailId = identityUser.Email,
            //                    Password = identityUser.PasswordHash ?? ""
            //                };
            //                var customerCart = new Cart
            //                {
            //                    CartId = Guid.NewGuid(),
            //                    CustomerId = Guid.Parse(identityUser.Id)
            //                };
            //                var createdCustomer = await customerRepositoryService.CreateCustomerAsync(customerCredential);
            //                await customerRepositoryService.CreateCartAsync(customerCart);
            //                return Ok(new { Message = localizer["UserRegistrationSuccess"].Value, createdCustomer.CustomerId, createdCustomer.EmailId });
            //            }
            //            await userManager.DeleteAsync(identityUser);
            //            return BadRequest(new { Message = localizer["RoleAdditionFailed"].Value, Error = identityResult.Errors.ToList() });
            //        }
            //        return BadRequest(new { Message = localizer["UserRegistrationFailure"].Value, Error = identityResult.Errors.ToList() });
            //    }
            //    return BadRequest(new { Message = localizer["EmptyRoles"].Value });
            //}
            //catch (Exception ex)
            //{
            //    await userManager.DeleteAsync(identityUser);
            //    return StatusCode(StatusCodes.Status500InternalServerError, (new
            //    {
            //        Error = ex.Message,
            //        Details = ex.ToString()
            //    }));
            //}

            try
            {
                if(registerRequestDto.Roles.Any() && registerRequestDto.Roles != null && registerRequestDto.Roles.Length > 0)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri("https://localhost:7025/");
                        var jsonData = JsonSerializer.Serialize(new { Email = registerRequestDto.Email, Password = registerRequestDto.Password });
                        var postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await httpClient.PostAsync("api/Auth/create-user", postData);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            var createUserResponse = JsonSerializer.Deserialize<CreateUserResponseDto>(responseString);
                            jsonData = JsonSerializer.Serialize(new { Email = registerRequestDto.Email, Roles = registerRequestDto.Roles });
                            postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage addingRolesResponse = await httpClient.PostAsync("api/Auth/add-roles", postData);
                            if (addingRolesResponse.IsSuccessStatusCode)
                            {
                                var customerCredential = new CustomerCredential
                                {
                                    CustomerId = createUserResponse.customerId,
                                    EmailId = createUserResponse.email,
                                    Password = createUserResponse.passwordHash ?? ""
                                };
                                var customerCart = new Cart
                                {
                                    CartId = Guid.NewGuid(),
                                    CustomerId = createUserResponse.customerId
                                };
                                var createdCustomer = await customerRepositoryService.CreateCustomerAsync(customerCredential);
                                await customerRepositoryService.CreateCartAsync(customerCart);
                                return Ok(new { Message = localizer["UserRegistrationSuccess"].Value, createdCustomer.CustomerId, createdCustomer.EmailId });
                            }
                            jsonData = JsonSerializer.Serialize(new { Email = registerRequestDto.Email });
                            postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage deleteUserResponse = await httpClient.PostAsync("api/Auth/delete-user", postData);
                            if (addingRolesResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                            {
                                var exceptionString = await addingRolesResponse.Content.ReadAsStringAsync();
                                var exceptionJson = JsonSerializer.Deserialize<ExceptionMessage>(exceptionString);
                                return BadRequest(new { Message = localizer["RoleAdditionFailed"].Value, Error = new List<Error> { new Error { description = exceptionJson.message} }});
                            }
                            var addRolesString = await addingRolesResponse.Content.ReadAsStringAsync();
                            var addRolesJson = JsonSerializer.Deserialize<UserCreationJson>(addRolesString);
                            return BadRequest(new { Message = localizer["RoleAdditionFailed"].Value, Error = addRolesJson.errors.ToList() });
                        }
                        var userCreationString = await response.Content.ReadAsStringAsync();
                        var userCreationJson = JsonSerializer.Deserialize<UserCreationJson>(userCreationString);
                        return BadRequest(new { Message = localizer["UserRegistrationFailure"].Value, Error = userCreationJson.errors.ToList() });
                    }
                }
                return BadRequest(new { Message = localizer["EmptyRoles"].Value });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Verify if the user is valid
        /// </summary>
        /// <param name="loginRequestDto">LoginRequestDto object</param>
        /// <returns>Returns a 2000k response with jwtToken, CustomerId and CustomerEmail as an object otherwise a 400BadRequest</returns>
        /// <response code="200">Returns the JwtToken, CustomerId, CustomerEmail when the login is successful</response>
        /// <response code="400">Returns Bad request with a message when the login is not successful</response>
        /// <response code="500">Returns Internal server error with Error and Details when an exception occurs</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            //try
            //{
            //    var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
            //    if (user != null)
            //    {
            //        var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            //        if (checkPasswordResult)
            //        {
            //            var roles = await userManager.GetRolesAsync(user);
            //            if (roles != null)
            //            {
            //                var jwtToken = tokenCreator.CreateJwtToken(user, roles.ToList());
            //                return Ok(new { jwtToken, CustomerId = user.Id, CustomerEmail = user.Email });
            //            }
            //        }
            //        return BadRequest(new { Message = localizer["InvalidPassword"].Value });
            //    }
            //    return BadRequest(new { Message = localizer["InvalidEmail"].Value });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, (new
            //    {
            //        Error = ex.Message,
            //        Details = ex.ToString()
            //    }));
            //}
            try
            {
                using(HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:7025/");
                    var jsonData = JsonSerializer.Serialize(new { Email = loginRequestDto.Email });
                    var postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("api/Auth/find-user-by-email", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonData = JsonSerializer.Serialize(loginRequestDto);
                        postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage passwordCheckResponse = await httpClient.PostAsync("api/Auth/check-password", postData);
                        if (passwordCheckResponse.IsSuccessStatusCode)
                        {
                            jsonData = JsonSerializer.Serialize(new { Email = loginRequestDto.Email });
                            postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage getRolesResponse = await httpClient.PostAsync("api/Auth/get-roles", postData);
                            if(getRolesResponse.IsSuccessStatusCode)
                            {
                                var rolesString = await getRolesResponse.Content.ReadAsStringAsync();
                                var roles = JsonSerializer.Deserialize<List<string>>(rolesString);
                                if(roles != null && roles.Count > 0)
                                {
                                    HttpResponseMessage idResponse = await httpClient.PostAsync("api/Auth/get-user-id", postData);
                                    var idString = await idResponse.Content.ReadAsStringAsync();
                                    var customer = JsonSerializer.Deserialize<CustomerIdEmailDto>(idString);
                                    if (idResponse.IsSuccessStatusCode)
                                    {
                                        var jwtToken = tokenCreator.CreateJwtToken(customer.email, roles);
                                        return Ok(new { jwtToken, CustomerId = customer.id, CustomerEmail = customer.email });
                                    }
                                }
                            }
                            return BadRequest(new { Message = "Something went wrong" });
                        }
                        return BadRequest(new { Message = localizer["InvalidPassword"].Value });
                    }
                    return BadRequest(new {Message = localizer["InvalidEmail"].Value});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Checks if the User with given Email exists
        /// </summary>
        /// <param name="details">GetUserByEmailDto object</param>
        /// <returns>Returns 200Ok response if the User exists otherwise 404NotFound</returns>
        /// <response code="200">Returns Ok response when the user is found</response>
        /// <response code="404">Returns Not found when the user is not found</response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
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
                return StatusCode(StatusCodes.Status500InternalServerError, (new
                {
                    ex.Message
                }));
            }
            //try
            //{
            //    using(HttpClient httpClient = new HttpClient())
            //    {
            //        httpClient.BaseAddress = new Uri("https://localhost:7025/");
            //        var jsonData = JsonSerializer.Serialize(new { Email = details.Email });
            //        var postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //        HttpResponseMessage response = await httpClient.PostAsync("api/Auth/find-user-by-email", postData);
            //        return response.IsSuccessStatusCode ? Ok() : response.StatusCode == System.Net.HttpStatusCode.NotFound ? NotFound() : StatusCode(StatusCodes.Status500InternalServerError);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            //}
        }

        /// <summary>
        /// Resets the password of the user
        /// </summary>
        /// <param name="newDetails">LoginRequestDto object</param>
        /// <returns>Returns 200Ok response if password is reset successfully otherwise 400BadRequest both the responses will have an object with Message key</returns>
        /// <response code="200">Returns Ok response with a message when the password is reset successfully</response>
        /// <response code="404">Returns Not found if the user with the given email is not found</response>
        /// <response code="400">Returns Bad request with message when an the password reset is not successful </response>
        /// <response code="500">Returns Internal Server Error with Message when an exception occurs</response>
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] LoginRequestDto newDetails)
        {
            //try
            //{
            //    var user = await userManager.FindByEmailAsync(newDetails.Email);
            //    var resultMessage = "";
            //    if (user == null)
            //    {
            //        resultMessage = localizer["InvalidEmail"].Value;
            //        return NotFound(new { Message = resultMessage });
            //    }
            //    var token = await userManager.GeneratePasswordResetTokenAsync(user);
            //    var result = await userManager.ResetPasswordAsync(user, token, newDetails.Password);
            //    resultMessage = result.Succeeded ? localizer["PasswordChangeSuccess"].Value : localizer["PasswordChangeFailure"].Value;
            //    return result.Succeeded ? Ok(new { Message = resultMessage }) : BadRequest(new { Message = resultMessage });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, (new
            //    {
            //        ex.Message
            //    }));
            //}
            try
            {
                var resultMessage = "";
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:7025/");
                    var jsonData = JsonSerializer.Serialize(new { Email = newDetails.Email });
                    var postData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("api/Auth/find-user-by-email", postData);
                    if (!response.IsSuccessStatusCode)
                    {
                        resultMessage = localizer["InvalidEmail"].Value;
                        return NotFound(new { Message = resultMessage });
                    }
                    var userData = JsonSerializer.Serialize(newDetails);
                    var postUserData = new StringContent(userData, Encoding.UTF8, "application/json");
                    HttpResponseMessage resetResponse = await httpClient.PutAsync("api/Auth/reset-password", postUserData);
                    resultMessage = resetResponse.IsSuccessStatusCode ? localizer["PasswordChangeSuccess"].Value : localizer["PasswordChangeFailure"].Value;
                    return resetResponse.IsSuccessStatusCode ? Ok(new { Message = resultMessage }) : BadRequest(new { Message = resultMessage });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

    }
}
