using FluentValidation.TestHelper;
using Service.Commands.EmployeeCafes;
using Service.Validators;
using System;

namespace Tests.Service.Validators
{
    public class UnassignEmployeeFromCafeCommandValidatorTests
    {
        private readonly UnassignEmployeeFromCafeCommandValidator validator;

        public UnassignEmployeeFromCafeCommandValidatorTests()
        {
            validator = new UnassignEmployeeFromCafeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_IdIsEmpty_Test()
        {
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = Guid.Empty };

            TestValidationResult<UnassignEmployeeFromCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_IdIsValid_Test()
        {
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = Guid.NewGuid() };

            TestValidationResult<UnassignEmployeeFromCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
        }
    }
} 