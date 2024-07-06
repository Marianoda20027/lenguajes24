using NUnit.Framework;
using Moq;
using storeapi.Controllers;
using storeapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using so.Models;

namespace UT
{
    public class SecurityTest
    {
        private AuthController authController;
        private Mock<IWebHostEnvironment> hostEnvironmentMock;

        [SetUp]
        public void Setup()
        {
            UserAccountSeeder.SeedUsers();
            hostEnvironmentMock = new Mock<IWebHostEnvironment>();
            hostEnvironmentMock.Setup(env => env.EnvironmentName).Returns("Development");
            authController = new AuthController(hostEnvironmentMock.Object);
        }

        [Test]
        public void Login_ValidUser_ReturnsOkObjectResult()
        {
            var validUser = new LoginModel("mariano", "123456");
            var result = authController.Login(validUser) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as AuthenticatedResponse;
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
        }

        [Test]
        public void Login_InvalidUser_ReturnsUnauthorizedResult()
        {
            var invalidUser = new LoginModel("invalid@gmail.com", "password");
            var result = authController.Login(invalidUser) as UnauthorizedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }
    }
}
