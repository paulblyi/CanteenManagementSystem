using System.ComponentModel.DataAnnotations;

namespace CanteenApi.DTOs
{
    public class TicketRequestDto
    {
        [Required]
        public DateTime TicketDate { get; set; }

        [Required]
        public string MealType { get; set; } = "Lunch";

        public string? Notes { get; set; }
    }

    public class TicketResponseDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime TicketDate { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public string? QRCode { get; set; }
    }

    public class TicketApprovalDto
    {
        public int TicketId { get; set; }
        public string Status { get; set; } = "Approved"; // Approved, Rejected
        public string? Notes { get; set; }
    }

    public class TicketRedemptionDto
    {
        [Required]
        public string TicketNumber { get; set; } = string.Empty;

        public string? VerificationMethod { get; set; } = "Manual";
    }
    public class RecentRedemptionDto
    {
        public string TicketNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime RedeemedAt { get; set; }
    }
}
