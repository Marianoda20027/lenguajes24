// UserAccountTests.cs
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using so.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
//PROYECTO12
namespace UT
{
    [TestFixture]
    public class UserAccountTests
    {
        [SetUp]
        public void Setup()
        {
            UserAccount.allUsers.Clear();
            
        }

        [Test]
        public void CreateUserAccount_WithValidData_ShouldAddUserToAllUsers()
        {
            // Arrange
            string userName = "testuser";
            string userPassword = "testpassword";
            var userRoles = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "TestRole")
            };

            // Act
            var userAccount = new UserAccount(userName, userPassword, userRoles);

            // Assert
            Assert.Contains(userAccount, UserAccount.allUsers);
        }

        [Test]
        public void CreateUserAccount_WithNullRoles_ShouldThrowArgumentException()
        {
            // Arrange
            string userName = "testuser";
            string userPassword = "testpassword";
            List<Claim> userRoles = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new UserAccount(userName, userPassword, userRoles));
            Assert.AreEqual("Debe crear roles para los usuarios", ex.Message);
        }

        [Test]
        public void CreateUserAccount_WithEmptyRoles_ShouldThrowArgumentException()
        {
            // Arrange
            string userName = "testuser";
            string userPassword = "testpassword";
            var userRoles = new List<Claim>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new UserAccount(userName, userPassword, userRoles));
            Assert.AreEqual("Debe crear roles para los usuarios", ex.Message);
        }

        [Test]
        public void CreateUserAccount_WithNullUserName_ShouldThrowArgumentException()
        {
            // Arrange
            string userName = null;
            string userPassword = "testpassword";
            var userRoles = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "TestRole")
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new UserAccount(userName, userPassword, userRoles));
            Assert.AreEqual("No pueden existir usuarios nulos", ex.Message);
        }

        [Test]
        public void CreateUserAccount_WithNullUserPassword_ShouldThrowArgumentException()
        {
            // Arrange
            string userName = "testuser";
            string userPassword = null;
            var userRoles = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "TestRole")
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new UserAccount(userName, userPassword, userRoles));
            Assert.AreEqual("Debe existir un password", ex.Message);
        }

        [Test]
        public void SeedUsers_ShouldAddUsersToAllUsers()
        {
            // Act
            UserAccountSeeder.SeedUsers();

            // Assert
            Assert.AreEqual(3, UserAccount.allUsers.Count);
            Assert.IsTrue(UserAccount.allUsers.Exists(u => u.UserName == "mariano"));
            Assert.IsTrue(UserAccount.allUsers.Exists(u => u.UserName == "juan"));
            Assert.IsTrue(UserAccount.allUsers.Exists(u => u.UserName == "sofia"));
        }

        [Test]
        public void SeedUsers_ValidatesUserRoles()
        {
            // Act
            UserAccountSeeder.SeedUsers();

            // Assert
            var mariano = UserAccount.allUsers.Find(u => u.UserName == "mariano");
            var juan = UserAccount.allUsers.Find(u => u.UserName == "juan");
            var sofia = UserAccount.allUsers.Find(u => u.UserName == "sofia");

            Assert.AreEqual(2, mariano.UserRoles.Count());
            Assert.AreEqual(2, juan.UserRoles.Count());
            Assert.AreEqual(2, sofia.UserRoles.Count());
        }
    }
}
