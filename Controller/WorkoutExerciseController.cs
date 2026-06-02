using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class WorkoutExerciseController
    {
        private readonly IWorkoutExerciseService _workoutExerciseService;

        public WorkoutExerciseController(IWorkoutExerciseService workoutExerciseService)
        {
            _workoutExerciseService = workoutExerciseService;
        }

        public async Task SaveOrderIndexChanges()
        {
            await _workoutExerciseService.SaveOrderIndexChangesAsync();
        }

        public async Task<WorkoutExercise> AddExerciseToWorkoutAsync(int workoutId, int exerciseId)
        {
            return await _workoutExerciseService.AddExerciseToWorkoutAsync(workoutId, exerciseId);
        }

        public async Task<WorkoutExercise> RemoveExerciseFromWorkoutAsync(int workoutId, int exerciseId)
        {
            return await _workoutExerciseService.RemoveExerciseFromWorkoutAsync(workoutId, exerciseId);
        }
    }
}
