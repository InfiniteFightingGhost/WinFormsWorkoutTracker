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
    public class ExerciseTests
    {
        private WorkoutDbContext _context;
        private Mock<IValidator<Exercise>> _validatorMock;
        private ExerciseService _service;
        private ExerciseController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _validatorMock = new Mock<IValidator<Exercise>>();
            _service = new ExerciseService(() => new WorkoutDbContext(options), _validatorMock.Object);
            _controller = new ExerciseController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsAllExercises()
        {
            // Arrange
            _context.Exercises.Add(new Exercise { Name = "E1", MuscleGroupId = 1, Description = "D", Instructions = "I" });
            _context.Exercises.Add(new Exercise { Name = "E2", MuscleGroupId = 1, Description = "D", Instructions = "I" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_Found_ReturnsExercise()
        {
            // Arrange
            var exercise = new Exercise { Name = "Pushup", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(exercise.Id);

            // Assert
            Assert.That(result?.Name, Is.EqualTo("Pushup"));
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
        public async Task CreateExercise_ValidExercise_SavesToDatabase()
        {
            // Arrange
            var exercise = new Exercise { Name = "Pushup", MuscleGroupId = 1, Description = "Test", Instructions = "Test" };
            _validatorMock.Setup(v => v.ValidateAsync(exercise, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.CreateAsync(exercise);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Pushup"));
            Assert.That(await _context.Exercises.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllByMuscleGroup_ReturnsCorrectExercises()
        {
            // Arrange
            _context.Exercises.Add(new Exercise { Name = "E1", MuscleGroupId = 1, Description = "Test", Instructions = "Test" });
            _context.Exercises.Add(new Exercise { Name = "E2", MuscleGroupId = 2, Description = "Test", Instructions = "Test" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllByMuscleGroupAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("E1"));
        }

        [Test]
        public async Task Update_Valid_UpdatesDatabase()
        {
            // Arrange
            var exercise = new Exercise { Name = "Old", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Exercise>(), default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.UpdateAsync(exercise.Id, "New", "New Desc");

            // Assert
            Assert.That(result.Name, Is.EqualTo("New"));
            Assert.That(result.Description, Is.EqualTo("New Desc"));
        }

        [Test]
        public void Update_NotFound_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateAsync(999, "N", "D"));
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var exercise = new Exercise { Name = "Delete", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteAsync(exercise.Id);

            // Assert
            Assert.That(await _context.Exercises.AnyAsync(e => e.Id == exercise.Id), Is.False);
        }

        [Test]
        public async Task Controller_GetAll_CallsService()
        {
            // Arrange
            _context.Exercises.Add(new Exercise { Name = "E1", MuscleGroupId = 1, Description = "D", Instructions = "I" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllExercisesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_GetById_CallsService()
        {
            // Arrange
            var exercise = new Exercise { Name = "E1", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetExerciseByIdAsync(exercise.Id);

            // Assert
            Assert.That(result?.Name, Is.EqualTo("E1"));
        }

        [Test]
        public async Task Controller_Update_CallsService()
        {
            // Arrange
            var exercise = new Exercise { Name = "Old", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Exercise>(), default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _controller.UpdateExerciseAsync(exercise.Id, "New", "Desc");

            // Assert
            Assert.That(result.Name, Is.EqualTo("New"));
        }

        [Test]
        public async Task Controller_Delete_CallsService()
        {
            // Arrange
            var exercise = new Exercise { Name = "Delete", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteExerciseAsync(exercise.Id);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Delete"));
            Assert.That(await _context.Exercises.CountAsync(), Is.EqualTo(0));
        }
    }
}
