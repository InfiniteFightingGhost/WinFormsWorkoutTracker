using Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IMuscleGroupService
    {
        Task<ICollection<MuscleGroup>> GetAllAsync();
        Task<MuscleGroup?> GetByIdAsync(int id);
        Task<MuscleGroup> CreateAsync(string name);
        Task<MuscleGroup> UpdateAsync(int id, string name);
        Task<MuscleGroup> DeleteAsync(int id);
    }
}
