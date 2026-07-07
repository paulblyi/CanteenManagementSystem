using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date = null)
        {
            try
            {
                var reportDate = date ?? DateTime.UtcNow.Date;
                var result = await _reportService.GetDailyReportAsync(reportDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily report");
                return StatusCode(500, new { message = "An error occurred retrieving daily report." });
            }
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var result = await _reportService.GetMonthlyReportAsync(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly report");
                return StatusCode(500, new { message = "An error occurred retrieving monthly report." });
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var result = await _reportService.GetDashboardStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { message = "An error occurred retrieving dashboard stats." });
            }
        }

        [HttpGet("department")]
        public async Task<IActionResult> GetDepartmentReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _reportService.GetDepartmentReportAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department report");
                return StatusCode(500, new { message = "An error occurred retrieving department report." });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var fileContents = await _reportService.ExportReportToExcelAsync(startDate, endDate);
                return File(fileContents, "text/csv", $"report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report");
                return StatusCode(500, new { message = "An error occurred exporting report." });
            }
        }
    }
}
