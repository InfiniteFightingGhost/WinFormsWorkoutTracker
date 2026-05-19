using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class WorkoutSessionController
    {
        WorkoutDbContext _context;
        AuthController _authController;
        public WorkoutSessionController(WorkoutDbContext context, AuthController authController)
        {
            _context = context;
            _authController = authController;
        }

        public async Task<ICollection<WorkoutSession>> GetAllAsync()
        {
            _authController.IsAuthenticated(Data.Enums.UserRole.User);
            return await _context.WorkoutSessions.ToListAsync();
        }

        public async Task<WorkoutSession?> GetByIdAsync(int id)
        {
            return await _context.WorkoutSessions.FindAsync(id);
        }

        public async Task<WorkoutSession> CreateAsync(WorkoutSession workoutSession)
        {
            _authController.IsAuthenticated(UserRole.User);
            _context.WorkoutSessions.Add(workoutSession);
            await _context.SaveChangesAsync();
            return workoutSession;
        }

        public async Task<ICollection<WorkoutSession>> GetAllUserSessionsAsync(int id)
        {
            return await _context.WorkoutSessions.Where(ws => ws.UserId == id).ToListAsync();
        }

        public ICollection<WorkoutSession> GetAllCurrentUserSessionsAsync()
        {
            return _authController.GetCurrentUser().Sessions;
        }

        public async Task<WorkoutSession> UpdateWorkoutSession(int id, DateTime end, string notes)
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
            if (notes == null)
            {
                throw new Exception("Notes cannot be null.");
            }
            session.End = end;
            session.Notes = notes;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> UpdateWorkoutSessionStatus(int id)
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

        public async Task<WorkoutSession> DeleteSessionAsync(int id)
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
    }
}
