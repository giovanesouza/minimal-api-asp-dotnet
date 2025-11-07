
using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enuns;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdministratorCrudRequestTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext) => Setup.ClassInit(testContext);
        [ClassCleanup]
        public static void ClassCleanup() => Setup.ClassCleanup();

        [TestInitialize]
        public void TestInit() => Setup.ClearAuthentication();

        [TestMethod]
        public async Task GetAllShouldReturnList()
        {
            // Arrange
            await Setup.AuthenticateAsAdmin();

            // Act
            var response = await Setup._client.GetAsync("/administrators");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetByIdShouldReturnAdministrator()
        {
            // Arrange
            await Setup.AuthenticateAsAdmin();

            // Act
            var response = await Setup._client.GetAsync("/administrators/1");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateShouldReturnForbiddenWhenNotAdmin()
        {
            // Arrange
            var newAdmin = new Administrator
            {
                Email = "forbidden@test.com",
                Password = "123456",
                Profile = "Viewer"
            };

            // Act
            await Setup.AuthenticateAsEditor();
            var content = new StringContent(JsonSerializer.Serialize(newAdmin), Encoding.UTF8, "application/json");
            var response = await Setup._client.PostAsync("/administrators", content);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateShouldAddNewAdministratorWhenAdmin()
        {
            // Arrange
            var newAdmin = new AdministratorDTO
            {
                Email = "created@test.com",
                Password = "123456",
                Profile = Profile.Editor // Possible profiles: Admin (0) or Editor (1)
            };

            // Act
            await Setup.AuthenticateAsAdmin();
            var content = new StringContent(JsonSerializer.Serialize(newAdmin), Encoding.UTF8, "application/json");
            var response = await Setup._client.PostAsync("/administrators", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
    }
}