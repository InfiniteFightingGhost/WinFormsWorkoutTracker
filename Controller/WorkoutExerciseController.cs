using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class WorkoutExerciseController
    {
        WorkoutDbContext _context;
        AuthController _auth;

        public WorkoutExerciseController(WorkoutDbContext context, AuthController auth)
        {
            _context = context;
            _auth = auth;
        }
        public async Task SaveOrderIndexChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<WorkoutExercise> AddExerciseToWorkoutAsync(int workoutId, int exerciseId)
        {
            var workout = await _context.WorkoutSessions
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == _auth.GetCurrentUser().Id);

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
            WorkoutExercise pair;
            if (workout.Exercises == null)
            {
                pair = new WorkoutExercise()
                {
                    WorkoutId = workoutId,
                    ExerciseId = exerciseId,
                    OrderIndex = 1
                };
                workout.Exercises = new List<WorkoutExercise>();
            }
            else
            {
                pair = new WorkoutExercise()
                {
                    WorkoutId = workoutId,
                    ExerciseId = exerciseId,
                    OrderIndex = (workout.Exercises ?? new List<WorkoutExercise>()).Count + 1
                };
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
