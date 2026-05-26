using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Entities;
using Data.Enums;
namespace Controller
{
    public class WorkoutSetController
    {
        WorkoutDbContext _context;
        public WorkoutSetController(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ExerciseSet>> GetWorkoutSetsAsync()
        {
            return await _context.ExerciseSets.ToListAsync();
        }

        public async Task<ExerciseSet> GetExerciseSetByIdAsync(int id)
        {
            return await _context.ExerciseSets.FindAsync(id);
        }

        public async Task<ExerciseSet> CreateExerciseSetAsync(int workoutId, int exerciseId)
        {
            var exerciseSet = new ExerciseSet()
            {
                WorkoutExerciseId = workoutId,
                ExerciseId = exerciseId,
                Repetitions = 0,
                Weight = 0,
                Completed = false,
                OrderIndex = await _context.ExerciseSets
                .Where(es => es.WorkoutExerciseId == workoutId && es.ExerciseId == exerciseId)
                .CountAsync(),
                SetType = SetType.Regular
            };
            _context.ExerciseSets.Add(exerciseSet);
            await _context.SaveChangesAsync();
            return exerciseSet;
        }

        public async Task UpdateExerciseSetAsync(ExerciseSet set)
        {
            var existing = await _context.ExerciseSets.FindAsync(set.Id);
            if (existing != null)
            {
                existing.Weight = set.Weight;
                existing.Repetitions = set.Repetitions;
                existing.Completed = set.Completed;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExerciseSetAsync(int id)
        {
            var existing = await _context.ExerciseSets.FindAsync(id);
            if (existing != null)
            {
                _context.ExerciseSets.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
