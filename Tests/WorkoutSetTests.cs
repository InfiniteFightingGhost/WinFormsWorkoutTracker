using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Controller;

namespace WorkoutTracker.Tests
{
    [TestFixture]
    public class WorkoutSetTests
    {
        private WorkoutDbContext _context;
        private WorkoutSetService _service;
        private WorkoutSetController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _service = new WorkoutSetService(() => new WorkoutDbContext(options));
            _controller = new WorkoutSetController(_service);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsAll()
        {
            // Arrange
            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_Found_ReturnsSet()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(set.Id);

            // Assert
            Assert.That(result?.Weight, Is.EqualTo(10));
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
        public async Task CreateAsync_SetsCorrectDefaultValues()
        {
            // Act
            var result = await _service.CreateAsync(1, 1);

            // Assert
            Assert.That(result.WorkoutExerciseId, Is.EqualTo(1));
            Assert.That(result.ExerciseId, Is.EqualTo(1));
            Assert.That(result.Repetitions, Is.EqualTo(0));
            Assert.That(result.Weight, Is.EqualTo(0));
            Assert.That(result.Completed, Is.False);
            Assert.That(result.OrderIndex, Is.EqualTo(0));
        }

        [Test]
        public async Task Update_Valid_UpdatesDatabase()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10, Completed = false };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();

            set.Weight = 20;
            set.Repetitions = 12;
            set.Completed = true;

            // Act
            await _service.UpdateAsync(set);

            // Assert
            var updated = await _context.ExerciseSets.FindAsync(set.Id);
            Assert.That(updated?.Weight, Is.EqualTo(20));
            Assert.That(updated?.Repetitions, Is.EqualTo(12));
            Assert.That(updated?.Completed, Is.True);
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteAsync(set.Id);

            // Assert
            Assert.That(await _context.ExerciseSets.AnyAsync(s => s.Id == set.Id), Is.False);
        }

        [Test]
        public async Task GetExerciseProgressAsync_CalculatesCorrectMaxWeightAndVolume()
        {
            // Arrange
            var userId = 1;
            var exerciseId = 1;
            var session = new WorkoutSession 
            { 
                UserId = userId, 
                Status = WorkoutStatus.Finished,
                Start = DateTime.Now.AddDays(-1),
                End = DateTime.Now.AddDays(-1).AddHours(1)
            };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            var workoutExercise = new WorkoutExercise { WorkoutId = session.Id, ExerciseId = exerciseId, OrderIndex = 0 };
            _context.WorkoutExercises.Add(workoutExercise);
            await _context.SaveChangesAsync();

            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = workoutExercise.WorkoutId, ExerciseId = exerciseId, Weight = 100, Repetitions = 5, Completed = true });
            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = workoutExercise.WorkoutId, ExerciseId = exerciseId, Weight = 120, Repetitions = 3, Completed = true });
            await _context.SaveChangesAsync();

            // Act
            var progress = (await _service.GetExerciseProgressAsync(userId, exerciseId)).ToList();

            // Assert
            Assert.That(progress.Count, Is.EqualTo(1));
            Assert.That(progress[0].MaxWeight, Is.EqualTo(120));
            Assert.That(progress[0].MaxVolume, Is.EqualTo(500)); // 100 * 5 = 500
        }

        [Test]
        public async Task GetUserPRsAsync_ReturnsPRs()
        {
            // Arrange
            var userId = 1;
            var exercise = new Exercise { Id = 1, Name = "Deadlift", MuscleGroupId = 1, Description = "D", Instructions = "I" };
            _context.Exercises.Add(exercise);

            var session = new WorkoutSession { UserId = userId, Status = WorkoutStatus.Finished, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            var we = new WorkoutExercise { WorkoutId = session.Id, ExerciseId = exercise.Id, OrderIndex = 0, Exercise = exercise, WorkoutSession = session };
            _context.WorkoutExercises.Add(we);
            await _context.SaveChangesAsync();

            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = we.WorkoutId, ExerciseId = exercise.Id, Weight = 200, Repetitions = 1, Completed = true });
            await _context.SaveChangesAsync();

            // Act
            var prs = (await _service.GetUserPRsAsync(userId)).ToList();

            // Assert
            Assert.That(prs.Count, Is.GreaterThan(0));
            var first = prs[0];
            var type = first.GetType();
            Assert.That(type.GetProperty("ExerciseName")?.GetValue(first), Is.EqualTo("Deadlift"));
            Assert.That(type.GetProperty("MaxWeight")?.GetValue(first), Is.EqualTo(200));
        }

        [Test]
        public async Task GetMuscleVolumeAsync_ReturnsAggregatedVolume()
        {
            // Arrange
            var userId = 1;
            var mg = new MuscleGroup { Id = 1, Name = "Chest" };
            _context.MuscleGroups.Add(mg);
            var ex = new Exercise { Id = 1, Name = "Bench", MuscleGroupId = 1, MainMuscleGroup = mg, Description = "D", Instructions = "I" };
            _context.Exercises.Add(ex);
            
            var session = new WorkoutSession { UserId = userId, Status = WorkoutStatus.Finished, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            var we = new WorkoutExercise { WorkoutId = session.Id, ExerciseId = ex.Id, OrderIndex = 0, Exercise = ex };
            _context.WorkoutExercises.Add(we);
            await _context.SaveChangesAsync();

            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = we.WorkoutId, ExerciseId = ex.Id, Weight = 100, Repetitions = 10, Completed = true });
            await _context.SaveChangesAsync();

            // Act
            var volume = (await _service.GetMuscleVolumeAsync(userId)).ToList();

            // Assert
            Assert.That(volume.Count, Is.EqualTo(1));
            var first = volume[0];
            var type = first.GetType();
            Assert.That(type.GetProperty("MuscleGroup")?.GetValue(first), Is.EqualTo("Chest"));
            Assert.That(type.GetProperty("TotalVolume")?.GetValue(first), Is.EqualTo(1000));
        }

        [Test]
        public async Task Controller_GetAll_CallsService()
        {
            // Arrange
            _context.ExerciseSets.Add(new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetWorkoutSetsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_GetById_CallsService()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetExerciseSetByIdAsync(set.Id);

            // Assert
            Assert.That(result?.Weight, Is.EqualTo(10));
        }

        [Test]
        public async Task Controller_Create_CallsService()
        {
            // Act
            var result = await _controller.CreateExerciseSetAsync(1, 1);

            // Assert
            Assert.That(result.WorkoutExerciseId, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_Update_CallsService()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();
            set.Weight = 20;

            // Act
            await _controller.UpdateExerciseSetAsync(set);

            // Assert
            var updated = await _context.ExerciseSets.FindAsync(set.Id);
            Assert.That(updated?.Weight, Is.EqualTo(20));
        }

        [Test]
        public async Task Controller_Delete_CallsService()
        {
            // Arrange
            var set = new ExerciseSet { WorkoutExerciseId = 1, ExerciseId = 1, Weight = 10, Repetitions = 10 };
            _context.ExerciseSets.Add(set);
            await _context.SaveChangesAsync();

            // Act
            await _controller.DeleteExerciseSetAsync(set.Id);

            // Assert
            Assert.That(await _context.ExerciseSets.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task Controller_GetProgress_CallsService()
        {
            // Act
            var result = await _controller.GetExerciseProgressAsync(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Controller_GetPRs_CallsService()
        {
            // Act
            var result = await _controller.GetUserPRsAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Controller_GetVolume_CallsService()
        {
            // Act
            var result = await _controller.GetMuscleVolumeAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
