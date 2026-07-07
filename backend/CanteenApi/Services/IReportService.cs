using CanteenApi.DTOs;

namespace CanteenApi.Services
{
    public interface IReportService
    {
        Task<DailyReportDto> GetDailyReportAsync(DateTime date);
        Task<MonthlyReportDto> GetMonthlyReportAsync(int year, int month);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<DepartmentReportDto>> GetDepartmentReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<byte[]> ExportReportToExcelAsync(DateTime startDate, DateTime endDate);
    }
}
