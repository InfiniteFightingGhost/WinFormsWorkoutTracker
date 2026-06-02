using WorkoutTracker.Data;
using WorkoutTracker.Data.DTOs;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Controller;

namespace WorkoutTracker.Tests
{
    [TestFixture]
    public class AuthTests
    {
        private WorkoutDbContext _context;
        private Mock<IValidator<CreateUserDTO>> _validatorMock;
        private AuthService _service;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _validatorMock = new Mock<IValidator<CreateUserDTO>>();
            _service = new AuthService(() => new WorkoutDbContext(options), _validatorMock.Object);
            _controller = new AuthController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Register_ValidUser_SavesToDatabase()
        {
            // Arrange
            var dto = new CreateUserDTO { Username = "testuser", Email = "test@test.com", Password = "password" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.That(result.Username, Is.EqualTo(dto.Username));
            Assert.That(await _context.Users.AnyAsync(u => u.Username == "testuser"), Is.True);
            Assert.That(_service.GetCurrentUser(), Is.Not.Null);
        }

        [Test]
        public async Task Register_DuplicateUsername_ThrowsException()
        {
            // Arrange
            _context.Users.Add(new User { Username = "existing", Email = "other@test.com", Password = "p" });
            await _context.SaveChangesAsync();
            var dto = new CreateUserDTO { Username = "existing", Email = "test@test.com", Password = "password" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.RegisterAsync(dto));
        }

        [Test]
        public async Task Register_DuplicateEmail_ThrowsException()
        {
            // Arrange
            _context.Users.Add(new User { Username = "other", Email = "existing@test.com", Password = "p" });
            await _context.SaveChangesAsync();
            var dto = new CreateUserDTO { Username = "testuser", Email = "existing@test.com", Password = "password" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.RegisterAsync(dto));
        }

        [Test]
        public async Task Login_ValidCredentials_SetsCurrentUser()
        {
            // Arrange
            var user = new User { Username = "user1", Password = "password1", Email = "user1@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.LoginAsync("user1", "password1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(_service.GetCurrentUser()?.Username, Is.EqualTo("user1"));
        }

        [Test]
        public async Task Login_UserNotFound_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.LoginAsync("nonexistent", "p"));
        }

        [Test]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            // Arrange
            var user = new User { Username = "user1", Password = "password1", Email = "user1@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.LoginAsync("user1", "wrongpassword"));
        }

        [Test]
        public async Task Logout_ClearsCurrentUser()
        {
            // Arrange
            var user = new User { Username = "user1", Password = "password1", Email = "user1@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _service.LoginAsync("user1", "password1");

            // Act
            _service.Logout();

            // Assert
            Assert.That(_service.GetCurrentUser(), Is.Null);
        }

        [Test]
        public void IsAuthenticated_NoUserLoggedIn_ThrowsUnauthorized()
        {
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _service.IsAuthenticated(UserRole.User));
        }

        [Test]
        public async Task IsAuthenticated_UserLoggedInWithCorrectRole_DoesNotThrow()
        {
            // Arrange
            var user = new User { Username = "admin", Password = "password", Email = "admin@test.com", Role = UserRole.Admin };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _service.LoginAsync("admin", "password");

            // Act & Assert
            Assert.DoesNotThrow(() => _service.IsAuthenticated(UserRole.Admin));
        }

        [Test]
        public async Task IsAuthenticated_WithId_MatchesId_DoesNotThrow()
        {
            // Arrange
            var user = new User { Id = 10, Username = "user", Password = "p", Email = "u@test.com", Role = UserRole.User };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _service.LoginAsync("user", "p");

            // Act & Assert
            Assert.DoesNotThrow(() => _service.IsAuthenticated(UserRole.Admin, 10));
        }

        [Test]
        public async Task IsAuthenticated_WithId_MismatchesIdButIsAdmin_DoesNotThrow()
        {
            // Arrange
            var admin = new User { Id = 1, Username = "admin", Password = "p", Email = "a@test.com", Role = UserRole.Admin };
            _context.Users.Add(admin);
            await _context.SaveChangesAsync();
            await _service.LoginAsync("admin", "p");

            // Act & Assert
            Assert.DoesNotThrow(() => _service.IsAuthenticated(UserRole.Admin, 99));
        }

        [Test]
        public async Task IsAuthenticated_WithId_MismatchesIdAndNotAdmin_ThrowsUnauthorized()
        {
            // Arrange
            var user = new User { Id = 10, Username = "user", Password = "p", Email = "u@test.com", Role = UserRole.User };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _service.LoginAsync("user", "p");

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _service.IsAuthenticated(UserRole.Admin, 99));
        }

        [Test]
        public async Task Controller_Register_CallsService()
        {
            // Arrange
            var dto = new CreateUserDTO { Username = "test", Email = "t@t.com", Password = "p" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.RegisterAsync(dto);

            // Assert
            Assert.That(result.Username, Is.EqualTo("test"));
        }

        [Test]
        public async Task Controller_Login_CallsService()
        {
            // Arrange
            var user = new User { Username = "test", Password = "p", Email = "t@t.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.LoginAsync("test", "p");

            // Assert
            Assert.That(result?.Username, Is.EqualTo("test"));
        }

        [Test]
        public async Task Controller_Logout_CallsService()
        {
            // Arrange
            var user = new User { Username = "test", Password = "p", Email = "t@t.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _controller.LoginAsync("test", "p");

            // Act
            _controller.Logout();

            // Assert
            Assert.That(_controller.GetCurrentUser(), Is.Null);
        }
    }
}
