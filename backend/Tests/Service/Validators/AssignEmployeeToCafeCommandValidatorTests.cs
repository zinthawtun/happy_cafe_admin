using FluentValidation.TestHelper;
using Service.Commands.EmployeeCafes;
using Service.Validators;
using System;

namespace Tests.Service.Validators
{
    public class AssignEmployeeToCafeCommandValidatorTests
    {
        private readonly AssignEmployeeToCafeCommandValidator validator;

        public AssignEmployeeToCafeCommandValidatorTests()
        {
            validator = new AssignEmployeeToCafeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_CafeIdIsEmpty_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.Empty, EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CafeId);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_CafeIdIsValid_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.CafeId);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmployeeIdIsEmpty_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "", AssignedDate = DateTime.UtcNow };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmployeeId);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmployeeIdIsValid_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmployeeId);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_AssignedDateIsEmpty_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = default };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_AssignedDateIsInFuture_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddDays(1) };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_AssignedDateIsCurrentDateTime_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddSeconds(-1) };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_AssignedDateIsInPast_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddDays(-1) };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand { CafeId = Guid.Empty, EmployeeId = "", AssignedDate = DateTime.UtcNow.AddDays(1) };

            TestValidationResult<AssignEmployeeToCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CafeId);
            result.ShouldHaveValidationErrorFor(c => c.EmployeeId);
            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }
    }
}