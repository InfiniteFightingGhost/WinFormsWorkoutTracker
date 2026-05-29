using Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IWorkoutSessionService
    {
        Task<ICollection<WorkoutSession>> GetAllAsync();
        Task<WorkoutSession?> GetByIdAsync(int id);
        Task<WorkoutSession> CreateAsync(WorkoutSession workoutSession);
        Task<ICollection<WorkoutSession>> GetAllUserSessionsAsync(int id);
        ICollection<WorkoutSession>? GetAllCurrentUserSessions();
        Task<WorkoutSession> UpdateAsync(int id, DateTime end, string? title, string? notes, string? photoUrl = null);
        Task<WorkoutSession> UpdateStatusAsync(int id);
        Task<WorkoutSession> DeleteAsync(int id);
        Task<WorkoutSession?> GetActiveSessionAsync(int userId);
    }
}
