using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenApi.Models
{
    public class Batch
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string BatchNumber { get; set; } = string.Empty;
        
        [Required]
        public int CreatedBy { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
        
        [MaxLength(50)]
        public string? Department { get; set; }
        
        [Required]
        public DateTime TicketDate { get; set; }
        
        [Required]
        public string MealType { get; set; } = "Lunch";
        
        public int TotalTickets { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Active"; // Active, Completed, Cancelled
        
        public virtual ICollection<MealTicket> Tickets { get; set; } = new List<MealTicket>();
    }
}
