using FluentValidation;
using Service.Commands.Employees;

namespace Service.Validators
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Employee name is required")
                .MaximumLength(100).WithMessage("Employee name cannot exceed 100 characters");

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address format")
                .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");
        }
    }
} 