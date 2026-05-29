using Data.Entities;
using WorkoutTracker.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controller
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
    }
}
