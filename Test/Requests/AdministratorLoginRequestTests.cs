using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdministratorLoginRequestTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup() => Setup.ClassCleanup();

        [TestMethod]
        public async Task LoginShouldReturnSuccess()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "admin@test.com",
                Password = "123456"
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
            var response = await Setup._client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Act
            var body = await response.Content.ReadAsStringAsync();
            var logged = JsonSerializer.Deserialize<LoggedAdmininistratorModelView>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.IsNotNull(logged);
            Assert.IsFalse(string.IsNullOrEmpty(logged.Token));
        }

        [TestMethod]
        public async Task LoginShouldReturnUnauthorizedWhenCredentialsInvalid()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "wrong@test.com",
                Password = "nope"
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
            var response = await Setup._client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}