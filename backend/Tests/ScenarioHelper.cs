using Business.Entities;
using Utilities;
using System.Text.RegularExpressions;
using Api.Models;
using Service.Queries.Cafes;
using Moq;
using Service.Interfaces;
using Infrastructure.FileManagement;

namespace Tests
{
    public class ScenarioHelper
    {
        public static Employee CreateEmployee(string name, string email, string phone, Gender gender)
        {
            string validPhone = phone;
            if (!Regex.IsMatch(phone, @"^[89]\d{7}$"))
            {
                validPhone = "8" + new string('0', 7);
            }
            
            return new Employee(
                UniqueIdGenerator.GenerateUniqueId(),
                name,
                email,
                validPhone,
                gender);
        }

        public static Cafe CreateCafe(string name, string description, string logo, string location)
        {
            return new Cafe(
                Guid.NewGuid(),
                name,
                description,
                logo,
                location);
        }

        public static CafeDto CreateTestCafeDto(Guid id, string name = "Test Cafe", string description = "A test cafe", 
            string location = "Test Location", string logo = "test-logo.png")
        {
            return new CafeDto
            {
                Id = id,
                Name = name,
                Description = description,
                Location = location,
                Logo = logo
            };
        }

        public static DeleteCafeModel CreateDeleteCafeModel(Guid id)
        {
            return new DeleteCafeModel { Id = id };
        }

        public static void SetupCafeServiceForDelete(Mock<ICafeService> cafeServiceMock, Guid cafeId, CafeDto cafeDto, bool deleteResult = true)
        {
            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafeDto);

            cafeServiceMock
                .Setup(s => s.DeleteAsync(cafeId))
                .ReturnsAsync(deleteResult);
        }

        public static void SetupFileServiceForLogoDelete(Mock<IFileService> fileServiceMock, string logoFileName, bool deleteResult)
        {
            fileServiceMock
                .Setup(s => s.DeleteLogo(logoFileName))
                .Returns(deleteResult);
        }
    }
}
