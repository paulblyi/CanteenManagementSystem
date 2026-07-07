using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenApi.Models
{
    public class MealTicket
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TicketNumber { get; set; } = string.Empty;
        
        public int? EmployeeId { get; set; }
        
        [ForeignKey("EmployeeId")]
        public virtual User? Employee { get; set; }
        
        [MaxLength(100)]
        public string? EmployeeName { get; set; }
        
        [MaxLength(50)]
        public string? Department { get; set; }
        
        [Required]
        public DateTime TicketDate { get; set; }
        
        [Required]
        public string MealType { get; set; } = "Lunch"; // Breakfast, Lunch, Dinner
        
        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Redeemed, Cancelled
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        
        public int? ApprovedBy { get; set; }
        
        [ForeignKey("ApprovedBy")]
        public virtual User? Approver { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
        
        [MaxLength(255)]
        public string? QRCode { get; set; }
        
        public string? Notes { get; set; }
        
        // For batch tickets
        public int? BatchId { get; set; }
        [ForeignKey("BatchId")]
        public virtual Batch? Batch { get; set; }
    }
}
