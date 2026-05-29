using Data.Entities;
using FluentValidation;

namespace WorkoutTracker.Service.Validators
{
    public class WorkoutValidator : AbstractValidator<Workout>
    {
        public WorkoutValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Workout title is required.")
                .MaximumLength(100).WithMessage("Workout title cannot exceed 100 characters.");
        }
    }
}
