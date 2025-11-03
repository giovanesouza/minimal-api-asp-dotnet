using MinimalApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void VehiclePropertiesShouldMatchValues()
        {
            // Arrange & Act
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Corolla",
                Brand = "Toyota",
                Year = 2022
            };

            // Assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Corolla", vehicle.Name);
            Assert.AreEqual("Toyota", vehicle.Brand);
            Assert.AreEqual(2022, vehicle.Year);
        }

        [TestMethod]
        public void VehicleValidationFailsWhenRequiredFieldsMissing()
        {
            // Arrange
            var vehicle = new Vehicle();

            var context = new ValidationContext(vehicle);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(vehicle, context, results, true);
            
            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void VehicleValidationFailsWhenNameTooLong()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = new string('A', 151),
                Brand = "Toyota",
                Year = 2022
            };

            var context = new ValidationContext(vehicle);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(vehicle, context, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Name")));
        }

        [TestMethod]
        public void VehicleValidationFailsWhenBrandTooLong()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "Corolla",
                Brand = new string('B', 101),
                Year = 2022
            };

            var context = new ValidationContext(vehicle);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(vehicle, context, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Brand")));
        }

        [TestMethod]
        public void VehicleValidationPassesWithValidData()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "Civic",
                Brand = "Honda",
                Year = 2023
            };

            var context = new ValidationContext(vehicle);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(vehicle, context, results, true);

            // Assert
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, results.Count);
        }
    }
}