using WorkoutTracker.Data.DTOs;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class WorkoutSetController
    {
        private readonly IWorkoutSetService _workoutSetService;

        public WorkoutSetController(IWorkoutSetService workoutSetService)
        {
            _workoutSetService = workoutSetService;
        }

        public async Task<IEnumerable<ExerciseProgressDTO>> GetExerciseProgressAsync(int userId, int exerciseId)
        {
            return await _workoutSetService.GetExerciseProgressAsync(userId, exerciseId);
        }

        public async Task<IEnumerable<UserPRDTO>> GetUserPRsAsync(int userId)
        {
            return await _workoutSetService.GetUserPRsAsync(userId);
        }

        public async Task<IEnumerable<MuscleVolumeDTO>> GetMuscleVolumeAsync(int userId)
        {
            return await _workoutSetService.GetMuscleVolumeAsync(userId);
        }

        public async Task<ICollection<ExerciseSet>> GetWorkoutSetsAsync()
        {
            return await _workoutSetService.GetAllAsync();
        }

        public async Task<ExerciseSet?> GetExerciseSetByIdAsync(int id)
        {
            return await _workoutSetService.GetByIdAsync(id);
        }

        public async Task<ExerciseSet> CreateExerciseSetAsync(int workoutId, int exerciseId)
        {
            return await _workoutSetService.CreateAsync(workoutId, exerciseId);
        }

        public async Task UpdateExerciseSetAsync(ExerciseSet set)
        {
            await _workoutSetService.UpdateAsync(set);
        }

        public async Task DeleteExerciseSetAsync(int id)
        {
            await _workoutSetService.DeleteAsync(id);
        }
    }
}
