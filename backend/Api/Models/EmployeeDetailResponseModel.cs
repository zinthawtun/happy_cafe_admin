namespace Api.Models
{
    public class EmployeeDetailResponseModel
    {
        public string Id { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string EmailAddress { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string Gender { get; set; } = string.Empty;
        
        public Guid? CafeId { get; set; }
        
        public string CafeName { get; set; } = string.Empty;
        
        public DateTime? StartDate { get; set; }
    }
} 