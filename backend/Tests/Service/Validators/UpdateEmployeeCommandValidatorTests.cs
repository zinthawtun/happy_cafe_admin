using Business.Entities;
using FluentValidation.TestHelper;
using Service.Commands.Employees;
using Service.Validators;
using System;

namespace Tests.Service.Validators
{
    public class UpdateEmployeeCommandValidatorTests
    {
        private readonly UpdateEmployeeCommandValidator validator;

        public UpdateEmployeeCommandValidatorTests()
        {
            validator = new UpdateEmployeeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_IdIsEmpty_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "", Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_IdIsValid_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsEmpty_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsTooLong_Test()
        {
            string longName = new string('A', 101);

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = longName, EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsValid_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Valid Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsExactlyMaxLength_Test()
        {
            string nameAtMaxLength = new string('A', 100);

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = nameAtMaxLength, EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsEmpty_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsTooLong_Test()
        {
            string longEmail = new string('a', 90) + "@example.com";

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = longEmail, Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsInvalidFormat_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "invalid-email", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmailAddressIsValid_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmailAddressIsExactlyMaxLength_Test()
        {
            string domain = "@example.com";
            string emailPrefix = new string('a', 100 - domain.Length);
            string emailAtMaxLength = emailPrefix + domain;

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = emailAtMaxLength, Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_PhoneIsEmpty_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = "", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_PhoneIsTooLong_Test()
        {
            string longPhone = new string('1', 21);

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = longPhone, Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_PhoneIsValid_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_PhoneIsExactlyMaxLength_Test()
        {
            string phoneAtMaxLength = new string('1', 20);

            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "employee1", Name = "Name", EmailAddress = "test@example.com", Phone = phoneAtMaxLength, Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            UpdateEmployeeCommand command = new UpdateEmployeeCommand { Id = "", Name = "", EmailAddress = "invalid-email", Phone = "", Gender = Gender.Male };

            TestValidationResult<UpdateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }
    }
} 