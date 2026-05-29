using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutExerciseService : IWorkoutExerciseService
    {
        private readonly WorkoutDbContext _context;
        private readonly IAuthService _auth;

        public WorkoutExerciseService(WorkoutDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task SaveOrderIndexChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<WorkoutExercise> AddExerciseToWorkoutAsync(int workoutId, int exerciseId)
        {
            var currentUser = _auth.GetCurrentUser();
            if (currentUser == null) throw new UnauthorizedAccessException();

            var workout = await _context.WorkoutSessions
                .Include(w => w.Exercises)
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == currentUser.Id);

            var exercise = await _context.Exercises.FindAsync(exerciseId);

            if (workout == null || exercise == null)
            {
                throw new Exception("Workout or Exercise not found.");
            }

            var existing = await _context.WorkoutExercises.FindAsync(workoutId, exerciseId);
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
            await _context.SaveChangesAsync();
            return pair;
        }

        public async Task<WorkoutExercise> RemoveExerciseFromWorkoutAsync(int workoutId, int exerciseId)
        {
            var pair = await _context.WorkoutExercises.FindAsync(workoutId, exerciseId);
            if (pair == null)
            {
                throw new Exception("This exercise is not part of the workout.");
            }
            _context.WorkoutExercises.Remove(pair);
            await _context.SaveChangesAsync();
            return pair;
        }
    }
}
