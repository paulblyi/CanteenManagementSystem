using CanteenApi.Data;
using CanteenApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CanteenApi.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly IReconciliationService _reconciliationService;

        public ReportService(AppDbContext context, IReconciliationService reconciliationService)
        {
            _context = context;
            _reconciliationService = reconciliationService;
        }

        public async Task<DailyReportDto> GetDailyReportAsync(DateTime date)
        {
            var reportDate = date.Date;
            
            // Get reconciliation for the date
            var reconciliation = await _reconciliationService.ReconcileDateAsync(reportDate);
            
            // Get ticket details
            var tickets = await _context.MealTickets
                .Include(t => t.Employee)
                .Where(t => t.TicketDate.Date == reportDate)
                .ToListAsync();

            // Get department breakdown
            var departmentBreakdown = tickets
                .Where(t => t.Status != "Cancelled")
                .GroupBy(t => t.Department ?? "Unknown")
                .Select(g => new DepartmentReportDto
                {
                    Department = g.Key,
                    TotalEmployees = g.Select(t => t.EmployeeId).Distinct().Count(),
                    TicketsIssued = g.Count(),
                    TicketsRedeemed = g.Count(t => t.Status == "Redeemed"),
                    TotalCost = g.Count(t => t.Status == "Redeemed") * 10.00m // Assuming $10 per meal
                })
                .ToList();

            // Get top employees (most meals)
            var topEmployees = tickets
                .Where(t => t.Status == "Redeemed")
                .GroupBy(t => new { t.EmployeeId, t.EmployeeName, t.Department })
                .Select(g => new EmployeeReportDto
                {
                    EmployeeId = g.Key.EmployeeId ?? 0,
                    EmployeeName = g.Key.EmployeeName ?? "Unknown",
                    Department = g.Key.Department ?? "Unknown",
                    TotalTickets = g.Count(),
                    RedeemedTickets = g.Count(),
                    TotalCost = g.Count() * 10.00m,
                    FavoriteMeal = g.GroupBy(t => t.MealType)
                        .OrderByDescending(grp => grp.Count())
                        .Select(grp => grp.Key)
                        .FirstOrDefault() ?? "None"
                })
                .OrderByDescending(e => e.TotalTickets)
                .Take(10)
                .ToList();

            return new DailyReportDto
            {
                Date = reportDate,
                TotalTicketsIssued = reconciliation.TotalTicketsIssued,
                TotalTicketsRedeemed = reconciliation.TotalTicketsRedeemed,
                TotalTicketsPending = reconciliation.TotalTicketsPending,
                TotalTicketsCancelled = tickets.Count(t => t.Status == "Cancelled"),
                
                BreakfastIssued = reconciliation.Breakfast.Issued,
                BreakfastRedeemed = reconciliation.Breakfast.Redeemed,
                BreakfastPending = reconciliation.Breakfast.Pending,
                
                LunchIssued = reconciliation.Lunch.Issued,
                LunchRedeemed = reconciliation.Lunch.Redeemed,
                LunchPending = reconciliation.Lunch.Pending,
                
                DinnerIssued = reconciliation.Dinner.Issued,
                DinnerRedeemed = reconciliation.Dinner.Redeemed,
                DinnerPending = reconciliation.Dinner.Pending,
                
                TotalRevenue = await CalculateDailyRevenueAsync(reportDate),
                BreakfastRevenue = await CalculateMealRevenueAsync(reportDate, "Breakfast"),
                LunchRevenue = await CalculateMealRevenueAsync(reportDate, "Lunch"),
                DinnerRevenue = await CalculateMealRevenueAsync(reportDate, "Dinner"),
                
                DepartmentBreakdown = departmentBreakdown,
                TopEmployees = topEmployees
            };
        }

        public async Task<MonthlyReportDto> GetMonthlyReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var dailyReports = new List<DailyReportDto>();
            var totalMeals = 0;
            var totalRevenue = 0m;
            var mealTypeCounts = new Dictionary<string, int>();
            var dailyMealCounts = new List<int>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dailyReport = await GetDailyReportAsync(date);
                dailyReports.Add(dailyReport);
                
                var dayMeals = dailyReport.TotalTicketsRedeemed;
                dailyMealCounts.Add(dayMeals);
                totalMeals += dayMeals;
                totalRevenue += dailyReport.TotalRevenue;
                
                mealTypeCounts["Breakfast"] = (mealTypeCounts.GetValueOrDefault("Breakfast") + dailyReport.BreakfastRedeemed);
                mealTypeCounts["Lunch"] = (mealTypeCounts.GetValueOrDefault("Lunch") + dailyReport.LunchRedeemed);
                mealTypeCounts["Dinner"] = (mealTypeCounts.GetValueOrDefault("Dinner") + dailyReport.DinnerRedeemed);
            }

            // Department summary for the month
            var departmentSummary = await GetDepartmentReportAsync(startDate, endDate);

            var busiestDay = dailyMealCounts.Any() ? dailyMealCounts.IndexOf(dailyMealCounts.Max()) + 1 : 0;
            var quietestDay = dailyMealCounts.Any() ? dailyMealCounts.IndexOf(dailyMealCounts.Min()) + 1 : 0;
            
            var mostPopularMeal = mealTypeCounts
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .FirstOrDefault() ?? "None";

            return new MonthlyReportDto
            {
                Year = year,
                Month = month,
                TotalDays = DateTime.DaysInMonth(year, month),
                TotalMealsServed = totalMeals,
                AverageDailyMeals = dailyMealCounts.Any() ? (int)Math.Round(dailyMealCounts.Average()) : 0,
                TotalRevenue = totalRevenue,
                AverageDailyRevenue = dailyMealCounts.Any() ? Math.Round(totalRevenue / dailyMealCounts.Count, 2) : 0,
                DailyReports = dailyReports,
                DepartmentSummary = departmentSummary,
                BusiestDay = busiestDay,
                QuietestDay = quietestDay,
                MostPopularMeal = mostPopularMeal
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // Today's stats
            var todayTickets = await _context.MealTickets
                .Where(t => t.TicketDate.Date == today)
                .ToListAsync();

            // Week stats
            var weekTickets = await _context.MealTickets
                .Where(t => t.TicketDate >= weekStart && t.TicketDate <= today)
                .ToListAsync();

            // Month stats
            var monthTickets = await _context.MealTickets
                .Where(t => t.TicketDate >= monthStart && t.TicketDate <= today)
                .ToListAsync();

            // Employee stats
            var totalEmployees = await _context.Users
                .CountAsync(u => u.Role == "Employee");
            
            var activeEmployees = await _context.Users
                .CountAsync(u => u.Role == "Employee" && u.IsActive);

            // Recent activities (last 10)
            var recentActivities = new List<RecentActivityDto>();

            // Add recent ticket requests
            var recentRequests = await _context.MealTickets
                .OrderByDescending(t => t.RequestedAt)
                .Take(5)
                .Select(t => new RecentActivityDto
                {
                    Timestamp = t.RequestedAt,
                    ActivityType = "Request",
                    Description = $"{t.EmployeeName} requested {t.MealType}",
                    User = t.EmployeeName ?? "Unknown"
                })
                .ToListAsync();
            recentActivities.AddRange(recentRequests);

            // Add recent redemptions
            var recentRedemptions = await _context.RedemptionLogs
                .Include(l => l.Employee)
                .OrderByDescending(l => l.RedemptionTime)
                .Take(5)
                .Select(l => new RecentActivityDto
                {
                    Timestamp = l.RedemptionTime,
                    ActivityType = "Redemption",
                    Description = $"{l.Employee!.FullName} redeemed ticket {l.TicketNumber}",
                    User = l.Employee!.FullName
                })
                .ToListAsync();
            recentActivities.AddRange(recentRedemptions);

            recentActivities = recentActivities
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();

            // Department stats
            var departmentStats = await _context.MealTickets
                .Where(t => t.TicketDate >= monthStart && t.TicketDate <= today)
                .GroupBy(t => t.Department ?? "Unknown")
                .Select(g => new DepartmentStatsDto
                {
                    Department = g.Key,
                    TicketsIssued = g.Count(),
                    TicketsRedeemed = g.Count(t => t.Status == "Redeemed"),
                    Cost = g.Count(t => t.Status == "Redeemed") * 10.00m
                })
                .ToListAsync();

            // Meal type stats
            var mealTypeStats = new MealTypeStatsDto
            {
                BreakfastCount = monthTickets.Count(t => t.MealType == "Breakfast" && t.Status == "Redeemed"),
                LunchCount = monthTickets.Count(t => t.MealType == "Lunch" && t.Status == "Redeemed"),
                DinnerCount = monthTickets.Count(t => t.MealType == "Dinner" && t.Status == "Redeemed"),
                BreakfastRevenue = monthTickets.Count(t => t.MealType == "Breakfast" && t.Status == "Redeemed") * 5.00m,
                LunchRevenue = monthTickets.Count(t => t.MealType == "Lunch" && t.Status == "Redeemed") * 10.00m,
                DinnerRevenue = monthTickets.Count(t => t.MealType == "Dinner" && t.Status == "Redeemed") * 8.00m
            };

            return new DashboardStatsDto
            {
                TodayTicketsIssued = todayTickets.Count(t => t.Status != "Cancelled"),
                TodayTicketsRedeemed = todayTickets.Count(t => t.Status == "Redeemed"),
                TodayPendingTickets = todayTickets.Count(t => t.Status == "Approved" || t.Status == "Pending"),
                TodayRevenue = todayTickets.Count(t => t.Status == "Redeemed") * 10.00m,
                
                WeekTicketsIssued = weekTickets.Count(t => t.Status != "Cancelled"),
                WeekTicketsRedeemed = weekTickets.Count(t => t.Status == "Redeemed"),
                WeekRevenue = weekTickets.Count(t => t.Status == "Redeemed") * 10.00m,
                
                MonthTicketsIssued = monthTickets.Count(t => t.Status != "Cancelled"),
                MonthTicketsRedeemed = monthTickets.Count(t => t.Status == "Redeemed"),
                MonthRevenue = monthTickets.Count(t => t.Status == "Redeemed") * 10.00m,
                
                ActiveEmployees = activeEmployees,
                TotalEmployees = totalEmployees,
                RecentActivities = recentActivities,
                DepartmentStats = departmentStats,
                MealTypeStats = mealTypeStats
            };
        }

        public async Task<List<DepartmentReportDto>> GetDepartmentReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.MealTickets
                .Include(t => t.Employee)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.TicketDate >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(t => t.TicketDate <= endDate.Value);

            var tickets = await query.ToListAsync();

            var departmentReport = tickets
                .Where(t => t.Status != "Cancelled")
                .GroupBy(t => t.Department ?? "Unknown")
                .Select(g => new DepartmentReportDto
                {
                    Department = g.Key,
                    TotalEmployees = g.Select(t => t.EmployeeId).Distinct().Count(),
                    TicketsIssued = g.Count(),
                    TicketsRedeemed = g.Count(t => t.Status == "Redeemed"),
                    TotalCost = g.Count(t => t.Status == "Redeemed") * 10.00m,
                    CostPerEmployee = g.Select(t => t.EmployeeId).Distinct().Count() > 0
                        ? Math.Round((g.Count(t => t.Status == "Redeemed") * 10.00m) / g.Select(t => t.EmployeeId).Distinct().Count(), 2)
                        : 0
                })
                .OrderByDescending(d => d.TicketsIssued)
                .ToList();

            return departmentReport;
        }

        public async Task<byte[]> ExportReportToExcelAsync(DateTime startDate, DateTime endDate)
        {
            // This is a placeholder - in production, use a library like EPPlus or ClosedXML
            // For now, return a simple CSV
            var report = await GetDailyReportAsync(startDate);
            var csv = $"Date,TotalIssued,TotalRedeemed,TotalPending,Breakfast,Lunch,Dinner,Revenue\n";
            csv += $"{startDate:yyyy-MM-dd},{report.TotalTicketsIssued},{report.TotalTicketsRedeemed},{report.TotalTicketsPending},{report.BreakfastRedeemed},{report.LunchRedeemed},{report.DinnerRedeemed},{report.TotalRevenue}\n";
            
            return System.Text.Encoding.UTF8.GetBytes(csv);
        }

        private async Task<decimal> CalculateDailyRevenueAsync(DateTime date)
        {
            const decimal breakfastPrice = 5.00m;
            const decimal lunchPrice = 10.00m;
            const decimal dinnerPrice = 8.00m;

            var tickets = await _context.MealTickets
                .Where(t => t.TicketDate.Date == date.Date && t.Status == "Redeemed")
                .ToListAsync();

            var revenue = tickets.Sum(t => t.MealType switch
            {
                "Breakfast" => breakfastPrice,
                "Lunch" => lunchPrice,
                "Dinner" => dinnerPrice,
                _ => 0
            });

            return revenue;
        }

        private async Task<decimal> CalculateMealRevenueAsync(DateTime date, string mealType)
        {
            var price = mealType switch
            {
                "Breakfast" => 5.00m,
                "Lunch" => 10.00m,
                "Dinner" => 8.00m,
                _ => 0
            };

            var count = await _context.MealTickets
                .CountAsync(t => t.TicketDate.Date == date.Date 
                    && t.MealType == mealType 
                    && t.Status == "Redeemed");

            return count * price;
        }
    }
}
