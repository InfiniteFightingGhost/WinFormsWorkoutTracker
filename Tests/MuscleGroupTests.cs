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
    public class MuscleGroupTests
    {
        private WorkoutDbContext _context;
        private Mock<IAuthService> _authMock;
        private MuscleGroupService _service;
        private MuscleGroupController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _authMock = new Mock<IAuthService>();
            _service = new MuscleGroupService(() => new WorkoutDbContext(options), _authMock.Object);
            _controller = new MuscleGroupController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsAllMuscleGroups()
        {
            // Arrange
            _context.MuscleGroups.Add(new MuscleGroup { Name = "Back" });
            _context.MuscleGroups.Add(new MuscleGroup { Name = "Legs" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_Found_ReturnsGroup()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Chest" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(group.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Name, Is.EqualTo("Chest"));
        }

        [Test]
        public async Task GetById_NotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Create_Valid_SavesToDatabase()
        {
            // Arrange
            string name = "Chest";
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            var result = await _service.CreateAsync(name);

            // Assert
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(await _context.MuscleGroups.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public void Create_EmptyName_ThrowsException()
        {
            // Arrange
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.CreateAsync(""));
        }

        [Test]
        public async Task Update_Valid_UpdatesDatabase()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Old Name" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            var result = await _service.UpdateAsync(group.Id, "New Name");

            // Assert
            Assert.That(result.Name, Is.EqualTo("New Name"));
            _context.ChangeTracker.Clear();
            var updated = await _context.MuscleGroups.FindAsync(group.Id);
            Assert.That(updated?.Name, Is.EqualTo("New Name"));
        }

        [Test]
        public void Update_NotFound_ThrowsException()
        {
            // Arrange
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateAsync(999, "Name"));
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Delete Me" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            await _service.DeleteAsync(group.Id);

            // Assert
            Assert.That(await _context.MuscleGroups.AnyAsync(m => m.Id == group.Id), Is.False);
        }

        [Test]
        public void Delete_NotFound_ThrowsException()
        {
            // Arrange
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.DeleteAsync(999));
        }

        [Test]
        public async Task Controller_GetMuscleGroups_CallsService()
        {
            // Arrange
            _context.MuscleGroups.Add(new MuscleGroup { Name = "Shoulders" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMuscleGroupsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_GetById_CallsService()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Shoulders" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMuscleGroupById(group.Id);

            // Assert
            Assert.That(result?.Name, Is.EqualTo("Shoulders"));
        }

        [Test]
        public async Task Controller_Create_CallsService()
        {
            // Arrange
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            var result = await _controller.CreateMuscleGroupAsync("New");

            // Assert
            Assert.That(result.Name, Is.EqualTo("New"));
        }

        [Test]
        public async Task Controller_Update_CallsService()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Old" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            var result = await _controller.UpdateMuscleGroupAsync(group.Id, "Updated");

            // Assert
            Assert.That(result.Name, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task Controller_Delete_CallsService()
        {
            // Arrange
            var group = new MuscleGroup { Name = "Delete" };
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();
            _authMock.Setup(x => x.IsAuthenticated(UserRole.Admin));

            // Act
            var result = await _controller.DeleteMuscleGroupAsync(group.Id);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Delete"));
            Assert.That(await _context.MuscleGroups.CountAsync(), Is.EqualTo(0));
        }
    }
}
