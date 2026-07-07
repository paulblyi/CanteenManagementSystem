using System.ComponentModel.DataAnnotations;

namespace CanteenApi.DTOs
{
    public class BatchCreateDto
    {
        [Required]
        public DateTime TicketDate { get; set; }
        
        [Required]
        public string MealType { get; set; } = "Lunch";
        
        [Required]
        public List<int> EmployeeIds { get; set; } = new List<int>();
        
        public string? Department { get; set; }
    }

    public class BatchResponseDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime TicketDate { get; set; }
        public string MealType { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<TicketResponseDto> Tickets { get; set; } = new List<TicketResponseDto>();
    }
}
