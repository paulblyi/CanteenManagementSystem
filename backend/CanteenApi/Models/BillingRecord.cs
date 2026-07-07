using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenApi.Models
{
    public class BillingRecord
    {
        public int Id { get; set; }
        
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime BillingPeriodStart { get; set; }
        
        [Required]
        public DateTime BillingPeriodEnd { get; set; }
        
        public int? DepartmentId { get; set; }
        
        [MaxLength(50)]
        public string? DepartmentName { get; set; }
        
        public int TotalTickets { get; set; } = 0;
        public int BreakfastCount { get; set; } = 0;
        public int LunchCount { get; set; } = 0;
        public int DinnerCount { get; set; } = 0;
        
        public decimal TotalAmount { get; set; } = 0;
        public decimal BreakfastAmount { get; set; } = 0;
        public decimal LunchAmount { get; set; } = 0;
        public decimal DinnerAmount { get; set; } = 0;
        
        public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        
        public int? CreatedBy { get; set; }
        
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
        
        public string? Notes { get; set; }
    }
}
