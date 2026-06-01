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
    public class WorkoutSessionTests
    {
        private WorkoutDbContext _context;
        private Mock<IAuthService> _authMock;
        private WorkoutSessionService _service;
        private WorkoutSessionController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new WorkoutDbContext(options);
            _authMock = new Mock<IAuthService>();
            _service = new WorkoutSessionService(_context, _authMock.Object);
            _controller = new WorkoutSessionController(_service);
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
            _authMock.Setup(a => a.IsAuthenticated(UserRole.User));
            _context.WorkoutSessions.Add(new WorkoutSession { UserId = 1, Start = DateTime.Now });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_Found_ReturnsSession()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(session.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
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
        public async Task CreateAsync_ValidSession_SavesToDatabase()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now, Status = WorkoutStatus.OnGoing };
            _authMock.Setup(x => x.IsAuthenticated(UserRole.User));

            // Act
            var result = await _service.CreateAsync(session);

            // Assert
            Assert.That(result.UserId, Is.EqualTo(1));
            Assert.That(await _context.WorkoutSessions.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllUserSessions_ReturnsCorrectSessions()
        {
            // Arrange
            _context.WorkoutSessions.Add(new WorkoutSession { UserId = 1, Start = DateTime.Now });
            _context.WorkoutSessions.Add(new WorkoutSession { UserId = 2, Start = DateTime.Now });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllUserSessionsAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserId, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_ValidEnd_FinishesSession()
        {
            // Arrange
            var start = DateTime.Now.AddHours(-1);
            var session = new WorkoutSession { UserId = 1, Start = start, Status = WorkoutStatus.OnGoing };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            var end = DateTime.Now;

            // Act
            var result = await _service.UpdateAsync(session.Id, end, "Finished Title", "Notes");

            // Assert
            Assert.That(result.Status, Is.EqualTo(WorkoutStatus.Finished));
            Assert.That(result.End, Is.EqualTo(end));
            Assert.That(result.Title, Is.EqualTo("Finished Title"));
        }

        [Test]
        public async Task UpdateAsync_EndBeforeStart_ThrowsException()
        {
            // Arrange
            var start = DateTime.Now;
            var session = new WorkoutSession { UserId = 1, Start = start, Status = WorkoutStatus.OnGoing };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            var end = start.AddMinutes(-1);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateAsync(session.Id, end, "Title", "Notes"));
        }

        [Test]
        public async Task UpdateStatus_Valid_UpdatesStatus()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now, Status = WorkoutStatus.OnGoing };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.UpdateStatusAsync(session.Id);

            // Assert
            Assert.That(result.Status, Is.EqualTo(WorkoutStatus.Finished));
        }

        [Test]
        public async Task Delete_Valid_RemovesFromDatabase()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteAsync(session.Id);

            // Assert
            Assert.That(await _context.WorkoutSessions.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetActiveSessionAsync_ReturnsCorrectSession()
        {
            // Arrange
            var userId = 1;
            var ongoing = new WorkoutSession { UserId = userId, Status = WorkoutStatus.OnGoing, Start = DateTime.Now };
            var finished = new WorkoutSession { UserId = userId, Status = WorkoutStatus.Finished, Start = DateTime.Now.AddDays(-1) };
            _context.WorkoutSessions.AddRange(ongoing, finished);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetActiveSessionAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(ongoing.Id));
        }

        [Test]
        public async Task Controller_GetAll_CallsService()
        {
            // Arrange
            _authMock.Setup(a => a.IsAuthenticated(UserRole.User));
            _context.WorkoutSessions.Add(new WorkoutSession { UserId = 1, Start = DateTime.Now });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_GetById_CallsService()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetByIdAsync(session.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Controller_GetAllUserSessions_CallsService()
        {
            // Arrange
            _context.WorkoutSessions.Add(new WorkoutSession { UserId = 1, Start = DateTime.Now });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllUserSessionsAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Controller_Update_CallsService()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now.AddHours(-1) };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateWorkoutSession(session.Id, DateTime.Now, "T", "N");

            // Assert
            Assert.That(result.Status, Is.EqualTo(WorkoutStatus.Finished));
        }

        [Test]
        public async Task Controller_Delete_CallsService()
        {
            // Arrange
            var session = new WorkoutSession { UserId = 1, Start = DateTime.Now };
            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            await _controller.DeleteSessionAsync(session.Id);

            // Assert
            Assert.That(await _context.WorkoutSessions.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task Controller_GetActiveSession_CallsService()
        {
            // Arrange
            var userId = 1;
            var ongoing = new WorkoutSession { UserId = userId, Status = WorkoutStatus.OnGoing, Start = DateTime.Now };
            _context.WorkoutSessions.Add(ongoing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetActiveSessionAsync(userId);

            // Assert
            Assert.That(result?.Id, Is.EqualTo(ongoing.Id));
        }
    }
}
