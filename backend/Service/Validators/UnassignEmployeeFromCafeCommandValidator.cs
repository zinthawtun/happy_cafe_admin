using FluentValidation;
using Service.Commands.EmployeeCafes;

namespace Service.Validators
{
    public class UnassignEmployeeFromCafeCommandValidator : AbstractValidator<UnassignEmployeeFromCafeCommand>
    {
        public UnassignEmployeeFromCafeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Assignment ID is required");
        }
    }
} 