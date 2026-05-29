using Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IExerciseService
    {
        Task<ICollection<Exercise>> GetAllAsync();
        Task<ICollection<Exercise>> GetAllByMuscleGroupAsync(int muscleGroupId);
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> CreateAsync(Exercise exercise);
        Task<Exercise> UpdateAsync(int id, string name, string description);
        Task<Exercise> DeleteAsync(int id);
    }
}
