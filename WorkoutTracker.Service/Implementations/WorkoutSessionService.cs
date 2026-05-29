using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly WorkoutDbContext _context;
        private readonly IAuthService _auth;

        public WorkoutSessionService(WorkoutDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<ICollection<WorkoutSession>> GetAllAsync()
        {
            _auth.IsAuthenticated(UserRole.User);
            return await _context.WorkoutSessions.ToListAsync();
        }

        public async Task<WorkoutSession?> GetByIdAsync(int id)
        {
            return await _context.WorkoutSessions.FindAsync(id);
        }

        public async Task<WorkoutSession> CreateAsync(WorkoutSession workoutSession)
        {
            _auth.IsAuthenticated(UserRole.User);
            _context.WorkoutSessions.Add(workoutSession);
            await _context.SaveChangesAsync();
            return workoutSession;
        }

        public async Task<ICollection<WorkoutSession>> GetAllUserSessionsAsync(int id)
        {
            return await _context.WorkoutSessions.Where(ws => ws.UserId == id).ToListAsync();
        }

        public ICollection<WorkoutSession>? GetAllCurrentUserSessions()
        {
            return _auth.GetCurrentUser()?.Sessions;
        }

        public async Task<WorkoutSession> UpdateAsync(int id, DateTime end, string? title, string? notes, string? photoUrl = null)
        {
            var session = await _context.WorkoutSessions.FindAsync(id);
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

            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> UpdateStatusAsync(int id)
        {
            var session = await _context.WorkoutSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception("Workout session not found.");
            }
            session.Status = WorkoutStatus.Finished;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> DeleteAsync(int id)
        {
            var session = await _context.WorkoutSessions.FindAsync(id);
            if (session == null)
            {
                throw new Exception("Workout session not found.");
            }
            _context.WorkoutSessions.Remove(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession?> GetActiveSessionAsync(int userId)
        {
            return await _context.WorkoutSessions
                .Include(w => w.Exercises)
                    .ThenInclude(we => we.Exercise)
                .Include(s => s.Exercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == WorkoutStatus.OnGoing);
        }
    }
}
