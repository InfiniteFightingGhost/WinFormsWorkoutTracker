using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class MuscleGroupController
    {
        private readonly IMuscleGroupService _muscleGroupService;

        public MuscleGroupController(IMuscleGroupService muscleGroupService)
        {
            _muscleGroupService = muscleGroupService;
        }

        public async Task<ICollection<MuscleGroup>> GetMuscleGroupsAsync()
        {
            return await _muscleGroupService.GetAllAsync();
        }

        public async Task<MuscleGroup?> GetMuscleGroupById(int id)
        {
            return await _muscleGroupService.GetByIdAsync(id);
        }

        public async Task<MuscleGroup> CreateMuscleGroupAsync(string name)
        {
            return await _muscleGroupService.CreateAsync(name);
        }

        public async Task<MuscleGroup> UpdateMuscleGroupAsync(int id, string name)
        {
            return await _muscleGroupService.UpdateAsync(id, name);
        }

        public async Task<MuscleGroup> DeleteMuscleGroupAsync(int id)
        {
            return await _muscleGroupService.DeleteAsync(id);
        }

        public async Task<ICollection<MuscleGroup>> BulkCreateAsync(ICollection<string> names)
        {
            return await _muscleGroupService.BulkCreateAsync(names);
        }
    }
}
