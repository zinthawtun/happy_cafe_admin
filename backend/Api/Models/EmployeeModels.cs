using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreateEmployeeModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        public Guid? CafeId { get; set; }
    }

    public class UpdateEmployeeModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        public Guid? CafeId { get; set; }
    }

    public class DeleteEmployeeModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;
    }

    public class EmployeeResponseModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int DaysWorked { get; set; }

        public string Cafe { get; set; } = string.Empty;
    }
} 