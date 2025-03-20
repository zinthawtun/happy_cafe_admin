using FluentValidation;
using Service.Commands.Cafes;

namespace Service.Validators
{
    public class UpdateCafeCommandValidator : AbstractValidator<UpdateCafeCommand>
    {
        public UpdateCafeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Cafe ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Cafe name is required")
                .MaximumLength(100).WithMessage("Cafe name cannot exceed 100 characters");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Logo)
                .MaximumLength(2000).WithMessage("Logo URL cannot exceed 2000 characters");
        }
    }
} 