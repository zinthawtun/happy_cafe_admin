namespace Api.Models
{
    public class CafeDetailResponseModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string Location { get; set; } = string.Empty;
        
        public string Logo { get; set; } = string.Empty;
        
        public int Employees { get; set; }
    }
} 