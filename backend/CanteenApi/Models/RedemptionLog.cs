using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenApi.Models
{
    public class RedemptionLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TicketNumber { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual User? Employee { get; set; }

        public int? ChefId { get; set; }

        [ForeignKey("ChefId")]
        public virtual User? Chef { get; set; }

        public DateTime RedemptionTime { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string? VerificationMethod { get; set; } // QR, Manual

        public string Status { get; set; } = "Success"; // Success, Failed
    }
}
