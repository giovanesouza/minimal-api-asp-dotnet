

using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class VehicleCrudRequestTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup() => Setup.ClassCleanup();

        [TestMethod]
        public async Task GetAllShouldReturnListWhenAdmin()
        {
            // Arrange
            await Setup.AuthenticateAsAdmin();
            // Act
            var response = await Setup._client.GetAsync("/vehicles");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.IsNotNull(vehicles);
            Assert.IsTrue(vehicles.Count >= 2);
        }

        [TestMethod]
        public async Task GetAllShouldReturnForbiddenWhenEditor()
        {
            // Arrange
            await Setup.AuthenticateAsEditor();
            
            // Act
            var response = await Setup._client.GetAsync("/vehicles");

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task GetByIdShouldReturnVehicleWhenEditor()
        {
            // Arrange
            await Setup.AuthenticateAsEditor();

            // Act
            var response = await Setup._client.GetAsync("/vehicles/1");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateShouldAddNewVehicleWhenAdminOrEditor()
        {
            // Arrange
            await Setup.AuthenticateAsEditor(); // Admin also permitted
            var newVehicle = new VehicleDTO
            {
                Name = "Fiesta",
                Brand = "Ford",
                Year = 2018
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(newVehicle), Encoding.UTF8, "application/json");
            var response = await Setup._client.PostAsync("/vehicles", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateShouldModifyVehicleWhenAdmin()
        {
            // Arrange
            await Setup.AuthenticateAsAdmin();
            var updateVehicle = new VehicleDTO
            {
                Name = "Corolla Updated",
                Brand = "Toyota",
                Year = 2021
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(updateVehicle), Encoding.UTF8, "application/json");
            var response = await Setup._client.PutAsync("/vehicles/1", content);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateShouldReturnForbiddenWhenEditor()
        {
            // Arrange
            await Setup.AuthenticateAsEditor();
            var updateVehicle = new VehicleDTO
            {
                Name = "Forbidden Update",
                Brand = "Ford",
                Year = 2020
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(updateVehicle), Encoding.UTF8, "application/json");
            var response = await Setup._client.PutAsync("/vehicles/1", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteShouldRemoveVehicleWhenAdmin()
        {
            // Arrange
            await Setup.AuthenticateAsAdmin();

            // Act
            var response = await Setup._client.DeleteAsync("/vehicles/1");

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteShouldReturnForbiddenWhenEditor()
        {
            // Arrange
            await Setup.AuthenticateAsEditor();

            // Act
            var response = await Setup._client.DeleteAsync("/vehicles/1");

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}