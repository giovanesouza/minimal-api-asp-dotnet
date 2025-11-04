using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace Test.Domain.Services
{
    [TestClass]
    public class VehicleServiceTests
    {
        private DBContext? _context;
        private VehicleService? _vehicleService;

        [TestInitialize]
        public void Setup()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 🔒 unique per test
                .Options;

            _context = new DBContext(options);
            
            _context.Database.EnsureDeleted(); // 🧹 clear any previous data
            _context.Database.EnsureCreated(); // recreate tables
    
            _vehicleService = new VehicleService(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context!.Database.EnsureDeleted(); // Clean database after each test
            _context.Dispose();
        }

        [TestMethod]
        public void ShouldCreateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Model S",
                Brand = "Tesla",
                Year = 2023
            };

            // Act
            var created = _vehicleService!.Create(vehicle);
            var found = _context!.Vehicles.Find(vehicle.Id);

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual(created.Name, found.Name);
        }

        [TestMethod]
        public void ShouldGetAllVehicles()
        {
            // Arrange
            _vehicleService!.Create(new Vehicle { Id = 1, Name = "Civic", Brand = "Honda", Year = 2021 });
            _vehicleService!.Create(new Vehicle { Id = 2, Name = "Corolla", Brand = "Toyota", Year = 2022 });

            // Act
            var result = _vehicleService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ShouldGetVehicleById()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Name = "Uno", Brand = "Fiat", Year = 2020 };
            _vehicleService!.Create(vehicle);

            // Act
            var found = _vehicleService.GetById(1);

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual(vehicle.Name, found!.Name);
        }

        [TestMethod]
        public void ShouldUpdateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Name = "Gol", Brand = "Volkswagen", Year = 2018 };
            _vehicleService!.Create(vehicle);

            // Act
            vehicle.Name = "Gol G7";
            vehicle.Year = 2020;
            var updated = _vehicleService.Update(vehicle);

            // Assert
            Assert.AreEqual("Gol G7", updated.Name);
            Assert.AreEqual(2020, updated.Year);
        }

        [TestMethod]
        public void ShouldDeleteVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Name = "Onix", Brand = "Chevrolet", Year = 2021 };
            _vehicleService!.Create(vehicle);

            // Act
            _vehicleService.Delete(vehicle);
            var result = _vehicleService.GetById(vehicle.Id);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldFilterVehiclesByName()
        {
            // Arrange
            _vehicleService!.Create(new Vehicle { Id = 1, Name = "Focus", Brand = "Ford", Year = 2019 });
            _vehicleService.Create(new Vehicle { Id = 2, Name = "Fiesta", Brand = "Ford", Year = 2018 });

            // Act
            var result = _vehicleService.GetAll(name: "Focus");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Focus", result.First().Name);
        }

        [TestMethod]
        public void ShouldFilterVehiclesByBrand()
        {
            // Arrange
            _vehicleService!.Create(new Vehicle { Id = 1, Name = "Focus", Brand = "Ford", Year = 2019 });
            _vehicleService.Create(new Vehicle { Id = 2, Name = "Civic", Brand = "Honda", Year = 2020 });

            // Act
            var result = _vehicleService.GetAll(brand: "Ford");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ford", result.First().Brand);
        }

        [TestMethod]
        public void ShouldReturnPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                _vehicleService!.Create(new Vehicle { Id = i, Name = $"Car {i}", Brand = "TestBrand", Year = 2020 });
            }

            // Act
            var page1 = _vehicleService!.GetAll(page: 1);
            var page2 = _vehicleService.GetAll(page: 2);

            // Assert
            Assert.AreEqual(10, page1.Count);
            Assert.AreEqual(5, page2.Count);
        }

    }
}
