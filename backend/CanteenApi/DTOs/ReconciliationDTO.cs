using System.ComponentModel.DataAnnotations;

namespace CanteenApi.DTOs
{
    public class ReconciliationSummaryDto
    {
        public int Id { get; set; }
        public DateTime ReconciliationDate { get; set; }
        public int TotalTicketsIssued { get; set; }
        public int TotalTicketsRedeemed { get; set; }
        public int TotalTicketsPending { get; set; }
        public int Variance { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ReconciledBy { get; set; }
        public DateTime? ReconciledAt { get; set; }
        public string? Notes { get; set; }
        
        // Breakdown by meal type
        public MealTypeBreakdownDto Breakfast { get; set; } = new MealTypeBreakdownDto();
        public MealTypeBreakdownDto Lunch { get; set; } = new MealTypeBreakdownDto();
        public MealTypeBreakdownDto Dinner { get; set; } = new MealTypeBreakdownDto();
        
        // Department breakdown
        public List<DepartmentBreakdownDto> DepartmentBreakdown { get; set; } = new List<DepartmentBreakdownDto>();
    }

    public class MealTypeBreakdownDto
    {
        public int Issued { get; set; }
        public int Redeemed { get; set; }
        public int Pending { get; set; }
        public int Variance { get; set; }
    }

    public class DepartmentBreakdownDto
    {
        public string Department { get; set; } = string.Empty;
        public int Issued { get; set; }
        public int Redeemed { get; set; }
        public int Pending { get; set; }
        public int Variance { get; set; }
    }

    public class ReconciliationCreateDto
    {
        [Required]
        public DateTime ReconciliationDate { get; set; }
        
        public string? Notes { get; set; }
    }

    public class ReconciliationCompleteDto
    {
        [Required]
        public int ReconciliationId { get; set; }
        
        public string? Notes { get; set; }
    }

    public class VarianceDetailDto
    {
        public string TicketNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public DateTime TicketDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string VarianceType { get; set; } = string.Empty; // Missing, Extra, Duplicate
    }
}
