using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IAuthService _auth;

        public WorkoutSessionService(Func<WorkoutDbContext> contextFactory, IAuthService auth)
        {
            _contextFactory = contextFactory;
            _auth = auth;
        }

        public async Task<ICollection<WorkoutSession>> GetAllAsync()
        {
            using var context = _contextFactory();
            return await context.WorkoutSessions.ToListAsync();
        }

        public async Task<WorkoutSession?> GetByIdAsync(int id)
        {
            using var context = _contextFactory();
            return await context.WorkoutSessions.FindAsync(id);
        }

        public async Task<WorkoutSession> CreateAsync(WorkoutSession workoutSession)
        {
            using var context = _contextFactory();
            context.WorkoutSessions.Add(workoutSession);
            await context.SaveChangesAsync();
            return workoutSession;
        }

        public async Task<ICollection<WorkoutSession>> GetAllUserSessionsAsync(int id)
        {
            using var context = _contextFactory();
            return await context.WorkoutSessions.Where(ws => ws.UserId == id).ToListAsync();
        }

        public ICollection<WorkoutSession>? GetAllCurrentUserSessions()
        {
            return _auth.GetCurrentUser()?.Sessions;
        }

        public async Task<WorkoutSession> UpdateAsync(int id, DateTime end, string? title, string? notes, string? photoUrl = null)
        {
            using var context = _contextFactory();
            var session = await context.WorkoutSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception("Workout session not found.");
            }
            if (end <= session.Start)
            {
                throw new Exception("Workout end must be after the start.");
            }

            session.End = end;
            session.Title = title;
            session.Notes = notes;
            session.PhotoUrl = photoUrl;
            session.Status = WorkoutStatus.Finished;

            await context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> UpdateStatusAsync(int id)
        {
            using var context = _contextFactory();
            var session = await context.WorkoutSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception("Workout session not found.");
            }
            session.Status = WorkoutStatus.Finished;
            await context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> DeleteAsync(int id)
        {
            using var context = _contextFactory();
            var session = await context.WorkoutSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception("Workout session not found.");
            }
            context.WorkoutSessions.Remove(session);
            await context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession?> GetActiveSessionAsync(int userId)
        {
            using var context = _contextFactory();
            return await context.WorkoutSessions
                .Include(w => w.Exercises)
                    .ThenInclude(we => we.Exercise)
                .Include(s => s.Exercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == WorkoutStatus.OnGoing);
        }
    }
}
