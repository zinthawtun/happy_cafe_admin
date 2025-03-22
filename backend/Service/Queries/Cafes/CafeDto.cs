namespace Service.Queries.Cafes
{
    public class CafeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Logo { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;
        
        public int EmployeeCount { get; set; }
    }
} 