using System.ComponentModel.DataAnnotations;

namespace CanteenApi.Models
{
    public class DailySummary
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime SummaryDate { get; set; }
        
        public int TotalMealsServed { get; set; } = 0;
        public int BreakfastCount { get; set; } = 0;
        public int LunchCount { get; set; } = 0;
        public int DinnerCount { get; set; } = 0;
        
        public decimal TotalRevenue { get; set; } = 0;
        public decimal BreakfastRevenue { get; set; } = 0;
        public decimal LunchRevenue { get; set; } = 0;
        public decimal DinnerRevenue { get; set; } = 0;
        
        public int TotalEmployees { get; set; } = 0;
        public int ActiveEmployees { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public string? Notes { get; set; }
    }
}
