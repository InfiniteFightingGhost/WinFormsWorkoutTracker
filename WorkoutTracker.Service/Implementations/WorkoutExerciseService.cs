using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutExerciseService : IWorkoutExerciseService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IAuthService _auth;

        public WorkoutExerciseService(Func<WorkoutDbContext> contextFactory, IAuthService auth)
        {
            _contextFactory = contextFactory;
            _auth = auth;
        }

        public async Task SaveOrderIndexChangesAsync()
        {
            // Note: This method seems to assume context is tracked across calls.
            // With context factory, it might need to be reconsidered if it's meant to save changes
            // made to entities retrieved from a different context instance.
            // However, looking at the usage in UI, it's likely used after some reordering logic.
            using var context = _contextFactory();
            await context.SaveChangesAsync();
        }

        public async Task<WorkoutExercise> AddExerciseToWorkoutAsync(int workoutId, int exerciseId)
        {
            var currentUser = _auth.GetCurrentUser();
            if (currentUser == null) throw new UnauthorizedAccessException();

            using var context = _contextFactory();
            var workout = await context.WorkoutSessions
                .Include(w => w.Exercises)
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == currentUser.Id);

            var exercise = await context.Exercises.FindAsync(exerciseId);

            if (workout == null || exercise == null)
            {
                throw new Exception("Workout or Exercise not found.");
            }

            var existing = await context.WorkoutExercises.FindAsync(workoutId, exerciseId);
            if (existing != null)
            {
                throw new Exception("This exercise is already added to the workout.");
            }

            var pair = new WorkoutExercise()
            {
                WorkoutId = workoutId,
                ExerciseId = exerciseId,
                OrderIndex = (workout.Exercises?.Count ?? 0) + 1
            };

            if (workout.Exercises == null)
            {
                workout.Exercises = new List<WorkoutExercise>();
            }

            workout.Exercises.Add(pair);
            await context.SaveChangesAsync();
            return pair;
        }

        public async Task<WorkoutExercise> RemoveExerciseFromWorkoutAsync(int workoutId, int exerciseId)
        {
            using var context = _contextFactory();
            var pair = await context.WorkoutExercises.FindAsync(workoutId, exerciseId);
            if (pair == null)
            {
                throw new Exception("This exercise is not part of the workout.");
            }
            context.WorkoutExercises.Remove(pair);
            await context.SaveChangesAsync();
            return pair;
        }
    }
}
