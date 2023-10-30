using AutoFixture;
using Castle.Core.Logging;
using ECommerce.Controllers;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Services.Interface;
using ECommerce.Tokens.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace ECommerce_WebAPI.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<UserManager<IdentityUser>> userManagerMock;
        private readonly Mock<ITokenCreator> tokenCreatorMock;
        private readonly Mock<ICustomerService> customerRepositoryServiceMock;
        private readonly Mock<IStringLocalizer<AuthController>> localizerMock;
        private readonly AuthController sut;

        public AuthControllerTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            var userStore = new Mock<IUserStore<IdentityUser>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            userManagerMock = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            tokenCreatorMock = new Mock<ITokenCreator>();
            customerRepositoryServiceMock = fixture.Freeze<Mock<ICustomerService>>();
            localizerMock = new Mock<IStringLocalizer<AuthController>>();
            sut = new AuthController(userManagerMock.Object, tokenCreatorMock.Object, localizerMock.Object, customerRepositoryServiceMock.Object);
        }

        //Signup Action method unit test cases
        [Fact]
        public async Task Signup_ShouldReturnOkResponse_WhenValidDetailsAreProvided()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            var customerCredential = fixture.Create<CustomerCredential>();

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>())).ReturnsAsync(IdentityResult.Success);
            customerRepositoryServiceMock.Setup(x => x.CreateCustomerAsync(It.IsAny<CustomerCredential>())).ReturnsAsync(customerCredential);
            localizerMock.Setup(loc => loc["UserRegistrationSuccess"]).Returns(new LocalizedString("UserRegistrationSuccess", "User registered successfully. Please Login"));

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<OkObjectResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task Signup_ShouldReturnBadRequest_WhenRolesAreNotProvided()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            registerRequestDto.Roles = Array.Empty<string>();
            var objectResult = new { Message = "Roles cannot be empty while registering a user. A default role 'Customer' must be assigned." };
            localizerMock.Setup(loc => loc["EmptyRoles"]).Returns(new LocalizedString("EmptyRoles", "Roles cannot be empty while registering a user. A default role 'Customer' must be assigned."));

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            var badResult = (BadRequestObjectResult)result;
            Assert.Equal(objectResult.ToString(), (badResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task Signup_ShouldReturnBadRequest_WhenProvidedDetailsAreNotValid()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already in use" }));
            localizerMock.Setup(loc => loc["UserRegistrationFailure"]).Returns(new LocalizedString("UserRegistrationFailure", "Unable to register user. Please go through the Error for details."));

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Signup_ShouldReturnBadRequest_WhenRoleAdditionFailedDueToSomeError()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Adding Roles Failed. Try registering once again" }));
            userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
            localizerMock.Setup(loc => loc["RoleAdditionFailed"]).Returns(new LocalizedString("RoleAdditionFailed", "Failed to add roles to the user. Please try registering again."));

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>()), Times.Once);
            userManagerMock.Verify(x => x.DeleteAsync(It.IsAny<IdentityUser>()), Times.Once);
        }

        [Fact]
        public async Task Signup_ShouldInternalServerError_WhenAnExceptionOccurs()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Throws<Exception>();

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = (ObjectResult)result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }

        //Login Action method unit test cases
        [Fact]
        public async Task Login_ShouldReturnOkResponse_WhenValidCredentialsAreProvided()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            var identityUser = fixture.Create<IdentityUser>();
            var roles = fixture.Create<List<string>>();
            var jwtToken = fixture.Create<string>();
            var CustomerId = identityUser.Id;
            var CustomerEmail = identityUser.Email;
            var objectResult = new { jwtToken, CustomerId, CustomerEmail };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(roles);
            tokenCreatorMock.Setup(x => x.CreateJwtToken(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(jwtToken);

            var result = await sut.Login(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<OkObjectResult>(result);
            result.Should().NotBeNull();
            var okResult = (OkObjectResult)result;
            Assert.Equal(objectResult.ToString(), (okResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenInValidPasswordIsProvided()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            var identityUser = fixture.Create<IdentityUser>();
            var objectResult = new { Message = "Incorrect Password..!!" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(false);
            localizerMock.Setup(loc => loc["InvalidPassword"]).Returns(new LocalizedString("InvalidPassword", "Incorrect Password..!!"));

            var result = await sut.Login(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            var badResult = (BadRequestObjectResult)result;
            Assert.Equal(objectResult.ToString(), (badResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenInValidEmailIsProvided()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            IdentityUser? identityUser = null;
            var objectResult = new { Message = "Invalid Email Entered..!!" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            localizerMock.Setup(loc => loc["InvalidEmail"]).Returns(new LocalizedString("InvalidEmail", "Invalid Email Entered..!!"));

            var result = await sut.Login(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            var badResult = (BadRequestObjectResult)result;
            Assert.Equal(objectResult.ToString(), (badResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Throws<Exception>();

            var result = await sut.Login(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = (ObjectResult)result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnOkResponse_WhenUserWithGivenEmailFound()
        {
            var getUserByEmail = fixture.Create<GetUserByEmailDto>();
            var identityUser = fixture.Create<IdentityUser>();
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

            var result = await sut.GetUserByEmail(getUserByEmail).ConfigureAwait(false);

            Assert.IsType<OkResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNotFound_WhenInvalidEmailIsProvided ()
        {
            var getUserByEmail = fixture.Create<GetUserByEmailDto>();
            IdentityUser? identityUser = null;

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

            var result = await sut.GetUserByEmail(getUserByEmail).ConfigureAwait(false);

            Assert.IsType<NotFoundResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            var getUserByEmail = fixture.Create<GetUserByEmailDto>();

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Throws<Exception>();

            var result = await sut.GetUserByEmail(getUserByEmail).ConfigureAwait(false);

            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = (ObjectResult)result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnOkResponse_WhenValidDataIsProvided()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            var identityUser = fixture.Create<IdentityUser>();
            var token = fixture.Create<string>();
            var resultObject = new { Message = "Password changed successfully. Please Login" };

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>())).ReturnsAsync(token);
            userManagerMock.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            localizerMock.Setup(loc => loc["PasswordChangeSuccess"]).Returns(new LocalizedString("PasswordChangeSuccess", "Password changed successfully. Please Login"));

            var result = await sut.ResetPassword(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<OkObjectResult>(result);
            result.Should().NotBeNull();
            var okResult = (OkObjectResult)result;
            Assert.Equal(resultObject.ToString(), (okResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenResetOperationFailed()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            var identityUser = fixture.Create<IdentityUser>();
            var token = fixture.Create<string>();
            var resultObject = new { Message = "Failed to reset password" };

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>())).ReturnsAsync(token);
            userManagerMock.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { }));
            localizerMock.Setup(loc => loc["PasswordChangeFailure"]).Returns(new LocalizedString("PasswordChangeFailure", "Failed to reset password"));

            var result = await sut.ResetPassword(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal(resultObject.ToString(), (badRequestResult.Value ?? "").ToString());
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnNotFound_WhenInvalidDataProvided()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            IdentityUser? identityUser = null;
            var resultObject = new { Message = "Invalid Email Entered..!!" };

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            localizerMock.Setup(loc => loc["InvalidEmail"]).Returns(new LocalizedString("InvalidEmail", "Invalid Email Entered..!!"));

            var result = await sut.ResetPassword(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<NotFoundObjectResult>(result);
            result.Should().NotBeNull();
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(resultObject.ToString(), (notFoundResult.Value ?? "").ToString());
        }
    }
}