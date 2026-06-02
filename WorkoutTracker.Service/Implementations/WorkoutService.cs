using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutService : IWorkoutService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IValidator<Workout> _validator;

        public WorkoutService(Func<WorkoutDbContext> contextFactory, IValidator<Workout> validator)
        {
            _contextFactory = contextFactory;
            _validator = validator;
        }

        public async Task<ICollection<Workout>> GetAllAsync()
        {
            using var context = _contextFactory();
            return await context.Workouts.ToListAsync();
        }

        public async Task<Workout?> GetByIdAsync(int id)
        {
            using var context = _contextFactory();
            return await context.Workouts.FindAsync(id);
        }

        public async Task<Workout> CreateAsync(Workout workout)
        {
            var validationResult = await _validator.ValidateAsync(workout);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            using var context = _contextFactory();
            context.Workouts.Add(workout);
            await context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> UpdateAsync(int id, string title, string description)
        {
            using var context = _contextFactory();
            var workout = await context.Workouts.FindAsync(id);
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

            await context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> DeleteAsync(int id)
        {
            using var context = _contextFactory();
            var workout = await context.Workouts.FindAsync(id);
            if (workout == null)
            {
                throw new Exception("Workout not found.");
            }
            context.Workouts.Remove(workout);
            await context.SaveChangesAsync();
            return workout;
        }
    }
}

