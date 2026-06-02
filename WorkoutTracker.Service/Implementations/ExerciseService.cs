using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class ExerciseService : IExerciseService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IValidator<Exercise> _validator;

        public ExerciseService(Func<WorkoutDbContext> contextFactory, IValidator<Exercise> validator)
        {
            _contextFactory = contextFactory;
            _validator = validator;
        }

        public async Task<ICollection<Exercise>> GetAllAsync()
        {
            using var context = _contextFactory();
            return await context.Exercises.ToListAsync();
        }

        public async Task<ICollection<Exercise>> GetAllByMuscleGroupAsync(int muscleGroupId)
        {
            using var context = _contextFactory();
            return await context.Exercises
                .Where(e => e.MuscleGroupId == muscleGroupId)
                .ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            using var context = _contextFactory();
            return await context.Exercises.FindAsync(id);
        }

        public async Task<Exercise> CreateAsync(Exercise exercise)
        {
            var validationResult = await _validator.ValidateAsync(exercise);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            using var context = _contextFactory();
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> UpdateAsync(int id, string name, string description)
        {
            using var context = _contextFactory();
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                throw new Exception("Exercise not found.");
            }

            exercise.Name = name;
            exercise.Description = description;

            var validationResult = await _validator.ValidateAsync(exercise);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> DeleteAsync(int id)
        {
            using var context = _contextFactory();
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                throw new Exception("Exercise not found.");
            }
            context.Exercises.Remove(exercise);
            await context.SaveChangesAsync();
            return exercise;
        }

        public async Task<ICollection<Exercise>> GetExercisesWithFiltration(ICollection<int> muscleGroupIds, string name)
        {
            using var context = _contextFactory();
            var query = context.Exercises.AsQueryable();
            if(muscleGroupIds.Any())
            {
                query = query.Where(e => muscleGroupIds.Contains(e.MuscleGroupId));
            }
            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }
            return await query.ToListAsync();
        }

        public async Task<ICollection<Exercise>> BulkCreateAsync(ICollection<Exercise> exercises)
        {
            if(exercises.Count == 0)  return new List<Exercise>();

            using var context = _contextFactory();
            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();

            return exercises;
        }
    }
}
