using WorkoutTracker.Data.DTOs;
using FluentValidation;

namespace WorkoutTracker.Service.Validators
{
    public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");
        }
    }
}
