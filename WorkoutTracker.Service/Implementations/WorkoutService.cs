using Data;
using Data.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutService : IWorkoutService
    {
        private readonly WorkoutDbContext _context;
        private readonly IValidator<Workout> _validator;

        public WorkoutService(WorkoutDbContext context, IValidator<Workout> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ICollection<Workout>> GetAllAsync()
        {
            return await _context.Workouts.ToListAsync();
        }

        public async Task<Workout?> GetByIdAsync(int id)
        {
            return await _context.Workouts.FindAsync(id);
        }

        public async Task<Workout> CreateAsync(Workout workout)
        {
            var validationResult = await _validator.ValidateAsync(workout);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> UpdateAsync(int id, string title, string description)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                throw new Exception("Workout not found.");
            }

            workout.Title = title;
            workout.Description = description;

            var validationResult = await _validator.ValidateAsync(workout);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> DeleteAsync(int id)
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

