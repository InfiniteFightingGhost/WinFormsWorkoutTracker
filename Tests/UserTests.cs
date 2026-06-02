using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Service.Interfaces;
using WorkoutTracker.Controller;

namespace WorkoutTracker.Tests
{
    [TestFixture]
    public class UserTests
    {
        private WorkoutDbContext _context;
        private Mock<IAuthService> _authMock;
        private UserService _service;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _authMock = new Mock<IAuthService>();
            _service = new UserService(() => new WorkoutDbContext(options), _authMock.Object);
            _controller = new UserController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_AsAdmin_ReturnsAllUsers()
        {
            // Arrange
            _authMock.Setup(a => a.IsAuthenticated(UserRole.Admin));
            _context.Users.Add(new User { Username = "u1", Email = "e1@t.com", Password = "p" });
            _context.Users.Add(new User { Username = "u2", Email = "e2@t.com", Password = "p" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_Found_ReturnsUser()
        {
            // Arrange
            var user = new User { Username = "u1", Email = "e1@t.com", Password = "p" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(user.Id);

            // Assert
            Assert.That(result?.Username, Is.EqualTo("u1"));
        }

        [Test]
        public async Task UpdateHeight_Valid_UpdatesDatabase()
        {
            // Arrange
            var user = new User { Username = "u1", Email = "e1@t.com", Password = "p", Height = 170 };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _authMock.Setup(a => a.IsAuthenticated(UserRole.Admin, user.Id));

            // Act
            await _service.UpdateHeightAsync(user.Id, 180);

            // Assert
            _context.ChangeTracker.Clear();
            var updated = await _context.Users.FindAsync(user.Id);
            Assert.That(updated?.Height, Is.EqualTo(180));
        }

        [Test]
        public void UpdateHeight_InvalidValue_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateHeightAsync(1, -10));
        }

        [Test]
        public async Task UpdateProfile_Valid_UpdatesMultipleFields()
        {
            // Arrange
            var user = new User { Username = "u1", Email = "e1@t.com", Password = "p", Height = 170, Weight = 70, Gender = Gender.Male };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _authMock.Setup(a => a.IsAuthenticated(UserRole.Admin, user.Id));

            // Act
            await _service.UpdateProfileAsync(user.Id, Gender.Female, 160, 55);

            // Assert
            _context.ChangeTracker.Clear();
            var updated = await _context.Users.FindAsync(user.Id);
            Assert.That(updated?.Gender, Is.EqualTo(Gender.Female));
            Assert.That(updated?.Height, Is.EqualTo(160));
            Assert.That(updated?.Weight, Is.EqualTo(55));
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var user = new User { Username = "u1", Email = "e1@t.com", Password = "p" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _authMock.Setup(a => a.IsAuthenticated(UserRole.Admin, user.Id));

            // Act
            await _service.DeleteAsync(user.Id);

            // Assert
            Assert.That(await _context.Users.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task Controller_UpdatePhoto_CallsService()
        {
            // Arrange
            var user = new User { Username = "u1", Email = "e1@t.com", Password = "p" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _authMock.Setup(a => a.IsAuthenticated(UserRole.Admin, user.Id));

            // Act
            await _controller.UpdatePhotoAsync(user.Id, "new_url");

            // Assert
            _context.ChangeTracker.Clear();
            var updated = await _context.Users.FindAsync(user.Id);
            Assert.That(updated?.PhotoUrl, Is.EqualTo("new_url"));
        }
    }
}
