using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Controller;

namespace WorkoutTracker.Tests
{
    [TestFixture]
    public class WorkoutTests
    {
        private WorkoutDbContext _context;
        private Mock<IValidator<Workout>> _validatorMock;
        private WorkoutService _service;
        private WorkoutController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _validatorMock = new Mock<IValidator<Workout>>();
            _service = new WorkoutService(() => new WorkoutDbContext(options), _validatorMock.Object);
            _controller = new WorkoutController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsAllWorkouts()
        {
            // Arrange
            _context.Workouts.Add(new Workout { Title = "W1", Description = "D" });
            _context.Workouts.Add(new Workout { Title = "W2", Description = "D" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_Found_ReturnsWorkout()
        {
            // Arrange
            var workout = new Workout { Title = "W1", Description = "D" };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(workout.Id);

            // Assert
            Assert.That(result?.Title, Is.EqualTo("W1"));
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
        public async Task CreateWorkout_ValidWorkout_SavesToDatabase()
        {
            // Arrange
            var workout = new Workout { Title = "Upper Body", Description = "Test" };
            _validatorMock.Setup(v => v.ValidateAsync(workout, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.CreateAsync(workout);

            // Assert
            Assert.That(result.Title, Is.EqualTo("Upper Body"));
            Assert.That(await _context.Workouts.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateWorkout_ValidData_UpdatesDatabase()
        {
            // Arrange
            var workout = new Workout { Title = "Old Title", Description = "Old Desc" };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Workout>(), default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.UpdateAsync(workout.Id, "New Title", "New Desc");

            // Assert
            Assert.That(result.Title, Is.EqualTo("New Title"));
            _context.ChangeTracker.Clear();
            var updatedWorkout = await _context.Workouts.FindAsync(workout.Id);
            Assert.That(updatedWorkout?.Title, Is.EqualTo("New Title"));
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var workout = new Workout { Title = "Delete", Description = "D" };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteAsync(workout.Id);

            // Assert
            Assert.That(await _context.Workouts.AnyAsync(w => w.Id == workout.Id), Is.False);
        }

        [Test]
        public void Delete_NotFound_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.DeleteAsync(999));
        }

        [Test]
        public async Task Controller_GetAllWorkouts_CallsService()
        {
            // Arrange
            _context.Workouts.Add(new Workout { Title = "W1", Description = "D1" });
            _context.Workouts.Add(new Workout { Title = "W2", Description = "D2" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetWorkoutsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Controller_GetById_CallsService()
        {
            // Arrange
            var workout = new Workout { Title = "W1", Description = "D" };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetWorkoutByIdAsync(workout.Id);

            // Assert
            Assert.That(result?.Title, Is.EqualTo("W1"));
        }

        [Test]
        public async Task Controller_Create_CallsService()
        {
            // Arrange
            var workout = new Workout { Title = "New", Description = "D" };
            _validatorMock.Setup(v => v.ValidateAsync(workout, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.CreateWorkoutAsync(workout);

            // Assert
            Assert.That(result.Title, Is.EqualTo("New"));
        }

        [Test]
        public async Task Controller_Delete_CallsService()
        {
            // Arrange
            var workout = new Workout { Title = "Del", Description = "D" };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteWorkoutAsync(workout.Id);

            // Assert
            Assert.That(result.Title, Is.EqualTo("Del"));
            Assert.That(await _context.Workouts.CountAsync(), Is.EqualTo(0));
        }
    }
}
