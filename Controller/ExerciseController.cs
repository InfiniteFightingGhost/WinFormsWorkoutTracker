using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class ExerciseController
    {
        private readonly IExerciseService _exerciseService;

        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<ICollection<Exercise>> GetAllExercisesAsync()
        {
            return await _exerciseService.GetAllAsync();
        }

        public async Task<ICollection<Exercise>> GetAllExercisesByMuscleGroup(int muscleGroupId)
        {
            return await _exerciseService.GetAllByMuscleGroupAsync(muscleGroupId);
        }

        public async Task<Exercise?> GetExerciseByIdAsync(int id)
        {
            return await _exerciseService.GetByIdAsync(id);
        }

        public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
        {
            return await _exerciseService.CreateAsync(exercise);
        }

        public async Task<Exercise> UpdateExerciseAsync(int id, string name, string description)
        {
            return await _exerciseService.UpdateAsync(id, name, description);
        }

        public async Task<Exercise> DeleteExerciseAsync(int id)
        {
            return await _exerciseService.DeleteAsync(id);
        }

        public async Task<ICollection<Exercise>> GetExercisesWithFiltration(ICollection<int> muscleGroupIds, string name)
        {
            return await _exerciseService.GetExercisesWithFiltration(muscleGroupIds, name);
        }

        public async Task<ICollection<Exercise>> BulkCreateExercisesAsync(ICollection<Exercise> exercises)
        {
            return await _exerciseService.BulkCreateAsync(exercises);
        }
    }
}
