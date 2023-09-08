using AutoFixture;
using Castle.Core.Logging;
using ECommerce.Controllers;
using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using ECommerce.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ECommerce_WebAPI.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<UserManager<IdentityUser>> userManagerMock;
        private readonly Mock<ITokenRepository> tokenRepositoryServiceMock;
        private readonly Mock<ICustomerRepository> customerRepositoryServiceMock;
        private readonly Mock<IStringLocalizer<AuthController>> localizerMock;
        private readonly AuthController sut;

        public AuthControllerTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            var userStore = new Mock<IUserStore<IdentityUser>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            userManagerMock = new Mock<UserManager<IdentityUser>>(userStore.Object,null, null, null, null, null, null, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            tokenRepositoryServiceMock = fixture.Freeze<Mock<ITokenRepository>>();
            customerRepositoryServiceMock = fixture.Freeze<Mock<ICustomerRepository>>();
            localizerMock = new Mock<IStringLocalizer<AuthController>>();
            sut = new AuthController(userManagerMock.Object, tokenRepositoryServiceMock.Object, localizerMock.Object, customerRepositoryServiceMock.Object);
        }

        //Signup Action method unit test cases
        [Fact]
        public async Task Signup_ShouldReturnOkResponse_WhenValidDetailsAreProvided()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            var customerCredential = fixture.Create<CustomerCredential>();

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>())).ReturnsAsync(IdentityResult.Success);
            customerRepositoryServiceMock.Setup(x => x.CreateCustomerAsync(customerCredential)).ReturnsAsync(customerCredential);
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
            registerRequestDto.Roles = new string[] { };
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
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already in use"}));
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
            userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<string[]>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Adding Roles Failed. Try registering once again"}));
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
        public async Task Signup_ShouldReturnBadRequest_WhenAnExceptionOccurs()
        {
            var registerRequestDto = fixture.Create<RegisterRequestDto>();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Throws<Exception>();

            var result = await sut.Signup(registerRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
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
            var objectResult = new { jwtToken };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(roles);
            tokenRepositoryServiceMock.Setup(x => x.CreateJwtToken(It.IsAny<IdentityUser>(),It.IsAny<List<string>>())).Returns(jwtToken);

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
        public async Task Login_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            var loginRequestDto = fixture.Create<LoginRequestDto>();
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Throws<Exception>();
            
            var result = await sut.Login(loginRequestDto).ConfigureAwait(false);

            Assert.IsType<BadRequestObjectResult>(result);
            result.Should().NotBeNull();
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }
    }
}