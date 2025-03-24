#nullable disable

using FluentValidation.TestHelper;
using Service.Commands.Cafes;
using Service.Validators;

namespace Tests.Service.Validators
{
    public class CreateCafeCommandValidatorTests
    {
        private readonly CreateCafeCommandValidator validator;

        public CreateCafeCommandValidatorTests()
        {
            validator = new CreateCafeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsEmpty_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "", Description = "Description", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsTooLong_Test()
        {
            string longName = new string('A', 101);

            CreateCafeCommand command = new CreateCafeCommand { Name = longName, Description = "Description", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsValid_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Valid Name", Description = "Description", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsExactlyMaxLength_Test()
        {
            string nameAtMaxLength = new string('A', 100);

            CreateCafeCommand command = new CreateCafeCommand { Name = nameAtMaxLength, Description = "Description", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsNull_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = null, Description = "Description", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsEmpty_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsTooLong_Test()
        {
            string longLocation = new string('A', 201);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = longLocation };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LocationIsValid_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "Valid Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LocationIsExactlyMaxLength_Test()
        {
            string locationAtMaxLength = new string('A', 200);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = locationAtMaxLength };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsNull_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = null };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsEmpty_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "", Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsNull_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = null, Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_DescriptionIsTooLong_Test()
        {
            string longDescription = new string('A', 501);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = longDescription, Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsExactlyMaxLength_Test()
        {
            string descriptionAtMaxLength = new string('A', 500);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = descriptionAtMaxLength, Location = "Location" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsEmpty_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "Location", Logo = "" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsNull_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "Location", Logo = null };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LogoIsTooLong_Test()
        {
            string longLogo = new string('A', 2001);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "Location", Logo = longLogo };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsExactlyMaxLength_Test()
        {
            string logoAtMaxLength = new string('A', 2000);

            CreateCafeCommand command = new CreateCafeCommand { Name = "Name", Description = "Description", Location = "Location", Logo = logoAtMaxLength };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand { Name = "", Description = "", Location = "" };

            TestValidationResult<CreateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.Location);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }
    }
} 