using System.ComponentModel.DataAnnotations;
using MinimalApi.Domain.Entities;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdmininistratorTest
    {
        [TestMethod]
        public void AdministratorPropertiesShouldMatchSetValues()
        {
            // Arrange & Act
            var admin = new Administrator
            {
                Id = 1,
                Email = "teste@teste.com",
                // Password = "teste",
                Profile = "admin"
            };

            // Assert
            Assert.AreEqual(1, admin.Id, "The Id property did not match the expected value.");
            Assert.AreEqual("teste@teste.com", admin.Email, "The Email property did not match the expected value.");
            // Assert.AreEqual("teste", admin.Password);
            Assert.AreEqual("admin", admin.Profile, "The Profile property did not match the expected value.");
        }

        [TestMethod]
        public void ShouldVerifyPasswordHash()
        {
            // Arrange
            const string password = "teste123";
            var admin = new Administrator
            {
                Password = BCryptNet.HashPassword(password)
            };

            // Act & Assert
            Assert.IsTrue(BCryptNet.Verify(password, admin.Password), "The password should match the hash.");
            Assert.IsFalse(BCryptNet.Verify("wrong_password", admin.Password), "The password should not match the hash.");
            Assert.AreEqual(60, admin.Password.Length); // BCrypt generates 60-char hashes
        }

        [TestMethod]
        public void AdministratorShouldFailValidationWhenRequiredFieldsAreMissing()
        {
            // Arrange
            var admin = new Administrator();

            var context = new ValidationContext(admin);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Validation should fail when required fields are missing.");
            Assert.AreEqual(3, results.Count, "Expected 3 validation errors for missing required fields.");
        }

        [TestMethod]
        public void AdministratorShouldFailValidationWhenEmailExceedsMaxLength()
        {
            // Arrange
            var admin = new Administrator
            {
                Email = new string('a', 256),
                Password = "123456",
                Profile = "admin"
            };

            var context = new ValidationContext(admin);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Validation should fail when Email exceeds max length.");
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email")), "Email should be flagged as invalid.");
        }

        [TestMethod]
        public void AdministratorShouldFailValidationWhenProfileExceedsMaxLength()
        {
            // Arrange
            var admin = new Administrator
            {
                Email = "admin@teste.com",
                Password = "123456",
                Profile = "administrator"
            };

            var context = new ValidationContext(admin);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Validation should fail when Profile exceeds max length.");
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Profile")), "Profile should be flagged as invalid.");
        }

        [TestMethod]
        public void AdministratorShouldPassValidationWhenAllFieldsAreValid()
        {
            // Arrange
            var admin = new Administrator
            {
                Email = "admin@teste.com",
                Password = "123456",
                Profile = "Admin"
            };

            var context = new ValidationContext(admin);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(admin, context, results, true);

            // Assert
            Assert.IsTrue(isValid, "Validation should pass with valid data.");
            Assert.AreEqual(0, results.Count, "No validation errors should be present.");
        }

    }
}