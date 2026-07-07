using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenApi.Models
{
    public class Reconciliation
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime ReconciliationDate { get; set; }
        
        public int TotalTicketsIssued { get; set; } = 0;
        public int TotalTicketsRedeemed { get; set; } = 0;
        public int TotalTicketsPending { get; set; } = 0;
        public int Variance { get; set; } = 0;
        
        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Variance
        
        public int? ReconciledBy { get; set; }
        
        [ForeignKey("ReconciledBy")]
        public virtual User? Reconciler { get; set; }
        
        public DateTime? ReconciledAt { get; set; }
        
        public string? Notes { get; set; }
        
        // Detailed breakdown
        public int BreakfastIssued { get; set; } = 0;
        public int BreakfastRedeemed { get; set; } = 0;
        public int BreakfastVariance { get; set; } = 0;
        
        public int LunchIssued { get; set; } = 0;
        public int LunchRedeemed { get; set; } = 0;
        public int LunchVariance { get; set; } = 0;
        
        public int DinnerIssued { get; set; } = 0;
        public int DinnerRedeemed { get; set; } = 0;
        public int DinnerVariance { get; set; } = 0;
        
        // Department-wise breakdown (stored as JSON or separate table)
        public string? DepartmentBreakdown { get; set; } // JSON string
    }
}
