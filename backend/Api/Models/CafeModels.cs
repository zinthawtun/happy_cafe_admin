using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreateCafeModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string Logo { get; set; } = string.Empty;
    }

    public class UpdateCafeModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string Logo { get; set; } = string.Empty;
    }

    public class DeleteCafeModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class CafeResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Employees { get; set; }

        public string Logo { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;
    }
} 