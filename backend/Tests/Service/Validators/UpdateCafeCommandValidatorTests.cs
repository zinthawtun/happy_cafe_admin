using FluentValidation.TestHelper;
using Service.Commands.Cafes;
using Service.Validators;

namespace Tests.Service.Validators
{
    public class UpdateCafeCommandValidatorTests
    {
        private readonly UpdateCafeCommandValidator validator;

        public UpdateCafeCommandValidatorTests()
        {
            validator = new UpdateCafeCommandValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_IdIsEmpty_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.Empty, Name = "Name", Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_IdIsValid_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsEmpty_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "", Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsTooLong_Test()
        {
            string longName = new string('A', 101);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = longName, Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsValid_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Valid Name", Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_NameIsExactlyMaxLength_Test()
        {
            string nameAtMaxLength = new string('A', 100);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = nameAtMaxLength, Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_NameIsNull_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = null, Description = "Description", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsEmpty_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsTooLong_Test()
        {
            string longLocation = new string('A', 201);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = longLocation };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LocationIsValid_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Valid Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LocationIsExactlyMaxLength_Test()
        {
            string locationAtMaxLength = new string('A', 200);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = locationAtMaxLength };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LocationIsNull_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = null };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Location);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsEmpty_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "", Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsNull_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = null, Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_DescriptionIsTooLong_Test()
        {
            string longDescription = new string('A', 501);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = longDescription, Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_DescriptionIsExactlyMaxLength_Test()
        {
            string descriptionAtMaxLength = new string('A', 500);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = descriptionAtMaxLength, Location = "Location" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsEmpty_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Location", Logo = "" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsNull_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Location", Logo = null };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhen_LogoIsTooLong_Test()
        {
            string longLogo = new string('A', 2001);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Location", Logo = longLogo };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldNotHaveErrorWhen_LogoIsExactlyMaxLength_Test()
        {
            string logoAtMaxLength = new string('A', 2000);

            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.NewGuid(), Name = "Name", Description = "Description", Location = "Location", Logo = logoAtMaxLength };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.Logo);
        }

        [Fact]
        public void Validator_ShouldValidateAllPropertiesAtOnce_Test()
        {
            UpdateCafeCommand command = new UpdateCafeCommand { Id = Guid.Empty, Name = "", Description = "", Location = "" };

            TestValidationResult<UpdateCafeCommand> result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.Location);

            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }
    }
} 