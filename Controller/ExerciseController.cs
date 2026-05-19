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
    public class ExerciseController
    {
        WorkoutDbContext _context;
        public ExerciseController(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Exercise>> GetAllExercisesAsync()
        {
            return await _context.Exercises.ToListAsync();
        }

        public async Task<ICollection<Exercise>> GetAllExercisesByMuscleGroup(int muscleGroupId)
        {
            return await _context.Exercises
                .Where(e => e.MuscleGroupId == muscleGroupId)
                .ToListAsync();
        }

        public async Task<Exercise?> GetExerciseByIdAsync(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }
        public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> UpdateExerciseAsync(int id, string name, string description)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                throw new Exception("Exercise not found.");
            }

            exercise.Name = name;
            exercise.Description = description;
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> DeleteExerciseAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                throw new Exception("Exercise not found.");
            }
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }
    }
}
