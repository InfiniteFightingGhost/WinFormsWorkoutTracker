using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IExerciseService
    {
        Task<ICollection<Exercise>> GetAllAsync();
        Task<ICollection<Exercise>> GetAllByMuscleGroupAsync(int muscleGroupId);
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> CreateAsync(Exercise exercise);
        Task<ICollection<Exercise>> BulkCreateAsync(ICollection<Exercise> exercises);
        Task<Exercise> UpdateAsync(int id, string name, string description);
        Task<Exercise> DeleteAsync(int id);
        Task<ICollection<Exercise>> GetExercisesWithFiltration(ICollection<int> muscleGroupIds, string name);
    }
}
