using WorkoutTracker.Data.DTOs;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IWorkoutSetService
    {
        Task<IEnumerable<ExerciseProgressDTO>> GetExerciseProgressAsync(int userId, int exerciseId);
        Task<IEnumerable<UserPRDTO>> GetUserPRsAsync(int userId);
        Task<IEnumerable<MuscleVolumeDTO>> GetMuscleVolumeAsync(int userId);
        Task<ICollection<ExerciseSet>> GetAllAsync();
        Task<ExerciseSet?> GetByIdAsync(int id);
        Task<ExerciseSet> CreateAsync(int workoutId, int exerciseId);
        Task UpdateAsync(ExerciseSet set);
        Task DeleteAsync(int id);
    }
}
