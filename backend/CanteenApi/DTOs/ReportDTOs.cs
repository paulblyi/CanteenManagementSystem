namespace CanteenApi.DTOs
{
    public class DailyReportDto
    {
        public DateTime Date { get; set; }
        public int TotalTicketsIssued { get; set; }
        public int TotalTicketsRedeemed { get; set; }
        public int TotalTicketsPending { get; set; }
        public int TotalTicketsCancelled { get; set; }
        
        public int BreakfastIssued { get; set; }
        public int BreakfastRedeemed { get; set; }
        public int BreakfastPending { get; set; }
        
        public int LunchIssued { get; set; }
        public int LunchRedeemed { get; set; }
        public int LunchPending { get; set; }
        
        public int DinnerIssued { get; set; }
        public int DinnerRedeemed { get; set; }
        public int DinnerPending { get; set; }
        
        public decimal TotalRevenue { get; set; }
        public decimal BreakfastRevenue { get; set; }
        public decimal LunchRevenue { get; set; }
        public decimal DinnerRevenue { get; set; }
        
        public List<DepartmentReportDto> DepartmentBreakdown { get; set; } = new List<DepartmentReportDto>();
        public List<EmployeeReportDto> TopEmployees { get; set; } = new List<EmployeeReportDto>();
    }

    public class DepartmentReportDto
    {
        public string Department { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int TicketsIssued { get; set; }
        public int TicketsRedeemed { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerEmployee { get; set; }
    }

    public class EmployeeReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public int RedeemedTickets { get; set; }
        public decimal TotalCost { get; set; }
        public string FavoriteMeal { get; set; } = string.Empty;
    }

    public class MonthlyReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalDays { get; set; }
        public int TotalMealsServed { get; set; }
        public int AverageDailyMeals { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDailyRevenue { get; set; }
        
        public List<DailyReportDto> DailyReports { get; set; } = new List<DailyReportDto>();
        public List<DepartmentReportDto> DepartmentSummary { get; set; } = new List<DepartmentReportDto>();
        
        public int BusiestDay { get; set; }
        public int QuietestDay { get; set; }
        public string MostPopularMeal { get; set; } = string.Empty;
    }

    public class DashboardStatsDto
    {
        public int TodayTicketsIssued { get; set; }
        public int TodayTicketsRedeemed { get; set; }
        public int TodayPendingTickets { get; set; }
        public decimal TodayRevenue { get; set; }
        
        public int WeekTicketsIssued { get; set; }
        public int WeekTicketsRedeemed { get; set; }
        public decimal WeekRevenue { get; set; }
        
        public int MonthTicketsIssued { get; set; }
        public int MonthTicketsRedeemed { get; set; }
        public decimal MonthRevenue { get; set; }
        
        public int ActiveEmployees { get; set; }
        public int TotalEmployees { get; set; }
        
        public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
        public List<DepartmentStatsDto> DepartmentStats { get; set; } = new List<DepartmentStatsDto>();
        public MealTypeStatsDto MealTypeStats { get; set; } = new MealTypeStatsDto();
    }

    public class RecentActivityDto
    {
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } = string.Empty; // Request, Approval, Redemption
        public string Description { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }

    public class DepartmentStatsDto
    {
        public string Department { get; set; } = string.Empty;
        public int TicketsIssued { get; set; }
        public int TicketsRedeemed { get; set; }
        public decimal Cost { get; set; }
    }

    public class MealTypeStatsDto
    {
        public int BreakfastCount { get; set; }
        public int LunchCount { get; set; }
        public int DinnerCount { get; set; }
        public decimal BreakfastRevenue { get; set; }
        public decimal LunchRevenue { get; set; }
        public decimal DinnerRevenue { get; set; }
    }
}
