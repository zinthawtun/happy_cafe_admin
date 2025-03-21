using FluentValidation.TestHelper;
using Service.Commands.EmployeeCafes;
using Service.Validators;
using System;

namespace Tests.Service.Validators
{
    public class UpdateEmployeeCafeAssignmentCommandValidatorTests
    {
        private readonly UpdateEmployeeCafeAssignmentCommandValidator validator;

        public UpdateEmployeeCafeAssignmentCommandValidatorTests()
        {
            validator = new UpdateEmployeeCafeAssignmentCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_IdIsEmpty_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.Empty, CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_IdIsValid_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_CafeIdIsEmpty_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.Empty, EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CafeId);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_CafeIdIsValid_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.CafeId);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmployeeIdIsEmpty_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmployeeId);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmployeeIdIsValid_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmployeeId);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_AssignedDateIsEmpty_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = default };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_AssignedDateIsInFuture_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddDays(1) };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_AssignedDateIsCurrentDateTime_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddSeconds(-1) };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_AssignedDateIsInPast_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.NewGuid(), CafeId = Guid.NewGuid(), EmployeeId = "employee1", AssignedDate = DateTime.UtcNow.AddDays(-1) };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.AssignedDate);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand { Id = Guid.Empty, CafeId = Guid.Empty, EmployeeId = "", AssignedDate = DateTime.UtcNow.AddDays(1) };

            TestValidationResult<UpdateEmployeeCafeAssignmentCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
            result.ShouldHaveValidationErrorFor(c => c.CafeId);
            result.ShouldHaveValidationErrorFor(c => c.EmployeeId);
            result.ShouldHaveValidationErrorFor(c => c.AssignedDate);
        }
    }
} 