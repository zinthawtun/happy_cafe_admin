using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreateEmployeeModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[89]\d{7}$", ErrorMessage = "Phone number must start with 8 or 9 and have exactly 8 digits")]
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
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        [RegularExpression(@"^[89]\d{7}$", ErrorMessage = "Phone number must start with 8 or 9 and have exactly 8 digits")]
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