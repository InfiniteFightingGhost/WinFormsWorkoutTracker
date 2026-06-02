using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class WorkoutSessionController
    {
        private readonly IWorkoutSessionService _workoutSessionService;

        public WorkoutSessionController(IWorkoutSessionService workoutSessionService)
        {
            _workoutSessionService = workoutSessionService;
        }

        public async Task<ICollection<WorkoutSession>> GetAllAsync()
        {
            return await _workoutSessionService.GetAllAsync();
        }

        public async Task<WorkoutSession?> GetByIdAsync(int id)
        {
            return await _workoutSessionService.GetByIdAsync(id);
        }

        public async Task<WorkoutSession> CreateAsync(WorkoutSession workoutSession)
        {
            return await _workoutSessionService.CreateAsync(workoutSession);
        }

        public async Task<ICollection<WorkoutSession>> GetAllUserSessionsAsync(int id)
        {
            return await _workoutSessionService.GetAllUserSessionsAsync(id);
        }

        public ICollection<WorkoutSession>? GetAllCurrentUserSessionsAsync()
        {
            return _workoutSessionService.GetAllCurrentUserSessions();
        }

        public async Task<WorkoutSession> UpdateWorkoutSession(int id, DateTime end, string? title, string? notes, string? photoUrl = null)
        {
            return await _workoutSessionService.UpdateAsync(id, end, title, notes, photoUrl);
        }

        public async Task<WorkoutSession> UpdateWorkoutSessionStatus(int id)
        {
            return await _workoutSessionService.UpdateStatusAsync(id);
        }

        public async Task<WorkoutSession> DeleteSessionAsync(int id)
        {
            return await _workoutSessionService.DeleteAsync(id);
        }

        public async Task<WorkoutSession?> GetActiveSessionAsync(int userId)
        {
            return await _workoutSessionService.GetActiveSessionAsync(userId);
        }
    }
}
