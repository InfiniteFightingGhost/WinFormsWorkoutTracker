using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IWorkoutService
    {
        Task<ICollection<Workout>> GetAllAsync();
        Task<Workout?> GetByIdAsync(int id);
        Task<Workout> CreateAsync(Workout workout);
        Task<Workout> UpdateAsync(int id, string title, string description);
        Task<Workout> DeleteAsync(int id);
    }
}
