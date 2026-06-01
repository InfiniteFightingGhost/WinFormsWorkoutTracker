using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Service.Interfaces;
using Controller;

namespace Tests
{
    [TestFixture]
    public class WorkoutExerciseTests
    {
        private WorkoutDbContext _context;
        private Mock<IAuthService> _authMock;
        private WorkoutExerciseService _service;
        private WorkoutExerciseController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _authMock = new Mock<IAuthService>();
            _service = new WorkoutExerciseService(_context, _authMock.Object);
            _controller = new WorkoutExerciseController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddExerciseToWorkout_ValidData_AddsRelationship()
        {
            // Arrange
            var user = new User { Id = 1, Username = "test", Email = "test@test.com", Password = "p" };
            _authMock.Setup(a => a.GetCurrentUser()).Returns(user);

            var session = new WorkoutSession { Id = 1, UserId = user.Id, Status = WorkoutStatus.OnGoing, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            var exercise = new Exercise { Id = 1, Name = "Press", Description = "D", Instructions = "I", MuscleGroupId = 1 };
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.AddExerciseToWorkoutAsync(session.Id, exercise.Id);

            // Assert
            Assert.That(result.WorkoutId, Is.EqualTo(session.Id));
            Assert.That(result.ExerciseId, Is.EqualTo(exercise.Id));
            Assert.That(await _context.WorkoutExercises.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task AddExerciseToWorkout_DuplicateExercise_ThrowsException()
        {
            // Arrange
            var user = new User { Id = 1, Username = "test", Email = "test@test.com", Password = "p" };
            _authMock.Setup(a => a.GetCurrentUser()).Returns(user);

            var session = new WorkoutSession { Id = 1, UserId = user.Id, Status = WorkoutStatus.OnGoing, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            var exercise = new Exercise { Id = 1, Name = "Press", Description = "D", Instructions = "I", MuscleGroupId = 1 };
            _context.Exercises.Add(exercise);
            
            var existing = new WorkoutExercise { WorkoutId = session.Id, ExerciseId = exercise.Id, OrderIndex = 1 };
            _context.WorkoutExercises.Add(existing);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.AddExerciseToWorkoutAsync(session.Id, exercise.Id));
        }

        [Test]
        public void AddExerciseToWorkout_NoUserLoggedIn_ThrowsUnauthorized()
        {
            // Arrange
            _authMock.Setup(a => a.GetCurrentUser()).Returns((User?)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _service.AddExerciseToWorkoutAsync(1, 1));
        }

        [Test]
        public async Task RemoveExerciseFromWorkout_Valid_RemovesRelationship()
        {
            // Arrange
            var pair = new WorkoutExercise { WorkoutId = 1, ExerciseId = 1, OrderIndex = 1 };
            _context.WorkoutExercises.Add(pair);
            await _context.SaveChangesAsync();

            // Act
            await _service.RemoveExerciseFromWorkoutAsync(1, 1);

            // Assert
            Assert.That(await _context.WorkoutExercises.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public void RemoveExerciseFromWorkout_NotFound_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.RemoveExerciseFromWorkoutAsync(1, 1));
        }

        [Test]
        public async Task Controller_Add_CallsService()
        {
            // Arrange
            var user = new User { Id = 1, Username = "test", Email = "test@test.com", Password = "p" };
            _authMock.Setup(a => a.GetCurrentUser()).Returns(user);
            _context.WorkoutSessions.Add(new WorkoutSession { Id = 1, UserId = 1 });
            _context.Exercises.Add(new Exercise { Id = 1, MuscleGroupId = 1, Name = "E", Description = "D", Instructions = "I" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.AddExerciseToWorkoutAsync(1, 1);

            // Assert
            Assert.That(result.WorkoutId, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_Remove_CallsService()
        {
            // Arrange
            _context.WorkoutExercises.Add(new WorkoutExercise { WorkoutId = 1, ExerciseId = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RemoveExerciseFromWorkoutAsync(1, 1);

            // Assert
            Assert.That(result.WorkoutId, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_SaveOrder_CallsService()
        {
            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _controller.SaveOrderIndexChanges());
        }
    }
}
