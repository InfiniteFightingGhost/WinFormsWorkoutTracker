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

        public async Task<ExerciseSet> CreateExerciseSetAsync(int workoutExerciseId)
        {
            var exerciseSet = new ExerciseSet()
            {
                WorkoutExerciseId = workoutExerciseId,
                Repetitions = 0,
                Weight = 0,
                Completed = false,
                OrderIndex = await _context.ExerciseSets
                .Where(es => es.WorkoutExerciseId == workoutExerciseId)
                .CountAsync(),
                SetType = SetType.Regular
            };
            _context.ExerciseSets.Add(exerciseSet);
            await _context.SaveChangesAsync();
            return exerciseSet;
        }
    }
}
