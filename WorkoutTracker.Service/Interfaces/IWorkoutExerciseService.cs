using Data.Entities;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IWorkoutExerciseService
    {
        Task SaveOrderIndexChangesAsync();
        Task<WorkoutExercise> AddExerciseToWorkoutAsync(int workoutId, int exerciseId);
        Task<WorkoutExercise> RemoveExerciseFromWorkoutAsync(int workoutId, int exerciseId);
    }
}
