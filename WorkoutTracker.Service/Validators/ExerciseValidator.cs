using Data.Entities;
using FluentValidation;

namespace WorkoutTracker.Service.Validators
{
    public class ExerciseValidator : AbstractValidator<Exercise>
    {
        public ExerciseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Exercise name is required.")
                .MaximumLength(100).WithMessage("Exercise name cannot exceed 100 characters.");

            RuleFor(x => x.MuscleGroupId)
                .GreaterThan(0).WithMessage("Muscle group is required.");
        }
    }
}
