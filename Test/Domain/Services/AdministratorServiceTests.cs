using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Domain.Services;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdministratorServiceTests
    {
        // Creates a new in-memory database context for testing
        private static DBContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            return new DBContext(options);
        }

        [TestMethod]
        public void ShouldSaveAdministrator()
        {
            // Arrange
            using var context = CreateTestDbContext();

            var admin = new Administrator
            {
                Email = "test@test.com",
                Password = "test",
                Profile = "Admin"
            };

            var service = new AdministratorService(context);

            // Act
            service.Create(admin);

            // Assert
            var allAdmins = service.GetAll(1);
            Assert.AreEqual(1, allAdmins.Count);
        }

        [TestMethod]
        public void ShouldFindAdministratorById()
        {
            // Arrange
            using var context = CreateTestDbContext();

            var admin = new Administrator
            {
                Email = "test@test.com",
                Password = "test",
                Profile = "Admin"
            };

            var service = new AdministratorService(context);

            // Act
            service.Create(admin);
            var result = service.GetById(admin.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(admin.Id, result?.Id);
        }

        /*
                [TestMethod]
                public void ShouldUpdateAdministrator()
                {
                    // Arrange
                    using var context = CreateTestDbContext();

                    var admin = new Administrator
                    {
                        Email = "update@test.com",
                        Password = "123456",
                        Profile = "Editor"
                    };

                    var service = new AdministratorService(context);
                    service.Create(admin);

                    // Act
                    admin.Profile = "Admin";
                    service.Update(admin);
                    var updated = service.GetById(admin.Id);

                    // Assert
                    Assert.AreEqual("Admin", updated.Profile);
                }
        */

        /*
                [TestMethod]
                public void ShouldDeleteAdministrator()
                {
                    // Arrange
                    using var context = CreateTestDbContext();

                    var admin = new Administrator
                    {
                        Email = "delete@test.com",
                        Password = "654321",
                        Profile = "Editor"
                    };

                    var service = new AdministratorService(context);
                    service.Create(admin);

                    // Act
                    service.Delete(admin.Id);
                    var deleted = service.GetById(admin.Id);

                    // Assert
                    Assert.IsNull(deleted);
                }
                */
    }
}
