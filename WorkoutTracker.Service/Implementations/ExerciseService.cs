using Data;
using Data.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class ExerciseService : IExerciseService
    {
        private readonly WorkoutDbContext _context;
        private readonly IValidator<Exercise> _validator;

        public ExerciseService(WorkoutDbContext context, IValidator<Exercise> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ICollection<Exercise>> GetAllAsync()
        {
            return await _context.Exercises.ToListAsync();
        }

        public async Task<ICollection<Exercise>> GetAllByMuscleGroupAsync(int muscleGroupId)
        {
            return await _context.Exercises
                .Where(e => e.MuscleGroupId == muscleGroupId)
                .ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }

        public async Task<Exercise> CreateAsync(Exercise exercise)
        {
            var validationResult = await _validator.ValidateAsync(exercise);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> UpdateAsync(int id, string name, string description)
        {
            var exercise = await _context.Exercises.FindAsync(id);
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

            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> DeleteAsync(int id)
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
