using FluentValidation;
using Service.Commands.EmployeeCafes;
using System;

namespace Service.Validators
{
    public class AssignEmployeeToCafeCommandValidator : AbstractValidator<AssignEmployeeToCafeCommand>
    {
        public AssignEmployeeToCafeCommandValidator()
        {
            RuleFor(x => x.CafeId)
                .NotEmpty().WithMessage("Cafe ID is required");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");

            RuleFor(x => x.AssignedDate)
                .NotEmpty().WithMessage("Assigned date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Assigned date cannot be in the future");
        }
    }
} 