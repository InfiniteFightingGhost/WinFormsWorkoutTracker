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
    public class WorkoutController
    {
        WorkoutDbContext _context;
        public WorkoutController(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Workout>> GetWorkoutsAsync()
        {
            return await _context.Workouts.ToListAsync();
        }

        public async Task<Workout?> GetWorkoutByIdAsync(int id)
        {
            return await _context.Workouts.FindAsync(id);
        }
        public async Task<Workout> CreateWorkoutAsync(Workout workout)
        {
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> UpdateWorkoutAsync(int id, string title, string description)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                throw new Exception("Workout not found.");
            }

            workout.Title = title;
            workout.Description = description;
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> DeleteWorkoutAsync(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                throw new Exception("Workout not found.");
            }
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return workout;
        }
    }
}
