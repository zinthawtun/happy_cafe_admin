using Business.Entities;
using FluentValidation.TestHelper;
using Service.Commands.Employees;
using Service.Validators;
using System;

namespace Tests.Service.Validators
{
    public class CreateEmployeeCommandValidatorTests
    {
        private readonly CreateEmployeeCommandValidator validator;

        public CreateEmployeeCommandValidatorTests()
        {
            validator = new CreateEmployeeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsEmpty_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsTooLong_Test()
        {
            string longName = new string('A', 101);

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = longName, EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsValid_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Valid Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsExactlyMaxLength_Test()
        {
            string nameAtMaxLength = new string('A', 100);

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = nameAtMaxLength, EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsEmpty_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsTooLong_Test()
        {
            string longEmail = new string('a', 90) + "@example.com";

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = longEmail, Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_EmailAddressIsInvalidFormat_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "invalid-email", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmailAddressIsValid_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_EmailAddressIsExactlyMaxLength_Test()
        {
            string domain = "@example.com";
            string emailPrefix = new string('a', 100 - domain.Length);
            string emailAtMaxLength = emailPrefix + domain;

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = emailAtMaxLength, Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.EmailAddress);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_PhoneIsEmpty_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "test@example.com", Phone = "", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_PhoneIsTooLong_Test()
        {
            string longPhone = new string('1', 21);

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "test@example.com", Phone = longPhone, Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_PhoneIsValid_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "test@example.com", Phone = "1234567890", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_PhoneIsExactlyMaxLength_Test()
        {
            string phoneAtMaxLength = new string('1', 20);

            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "Name", EmailAddress = "test@example.com", Phone = phoneAtMaxLength, Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand { Name = "", EmailAddress = "invalid-email", Phone = "", Gender = Gender.Male };

            TestValidationResult<CreateEmployeeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.EmailAddress);
            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }
    }
} 