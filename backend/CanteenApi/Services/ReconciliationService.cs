using System.Text.Json;
using CanteenApi.Data;
using CanteenApi.DTOs;
using CanteenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenApi.Services
{
    public class ReconciliationService : IReconciliationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReconciliationService> _logger;

        public ReconciliationService(AppDbContext context, ILogger<ReconciliationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReconciliationSummaryDto> CreateReconciliationAsync(DateTime date)
        {
            var reconciliationDate = date.Date;
            
            // Check if reconciliation already exists for this date
            var existing = await _context.Reconciliations
                .FirstOrDefaultAsync(r => r.ReconciliationDate.Date == reconciliationDate);
            
            if (existing != null)
                throw new InvalidOperationException("Reconciliation already exists for this date.");

            // Get all tickets for this date
            var tickets = await _context.MealTickets
                .Include(t => t.Employee)
                .Where(t => t.TicketDate.Date == reconciliationDate)
                .ToListAsync();

            // Calculate totals
            var totalIssued = tickets.Count(t => t.Status != "Cancelled");
            var totalRedeemed = tickets.Count(t => t.Status == "Redeemed");
            var totalPending = tickets.Count(t => t.Status == "Approved" || t.Status == "Pending");

            // Calculate by meal type
            var breakfastIssued = tickets.Count(t => t.MealType == "Breakfast" && t.Status != "Cancelled");
            var breakfastRedeemed = tickets.Count(t => t.MealType == "Breakfast" && t.Status == "Redeemed");
            
            var lunchIssued = tickets.Count(t => t.MealType == "Lunch" && t.Status != "Cancelled");
            var lunchRedeemed = tickets.Count(t => t.MealType == "Lunch" && t.Status == "Redeemed");
            
            var dinnerIssued = tickets.Count(t => t.MealType == "Dinner" && t.Status != "Cancelled");
            var dinnerRedeemed = tickets.Count(t => t.MealType == "Dinner" && t.Status == "Redeemed");

            // Calculate department breakdown
            var departmentBreakdown = tickets
                .Where(t => t.Status != "Cancelled")
                .GroupBy(t => t.Department ?? "Unknown")
                .Select(g => new DepartmentBreakdownDto
                {
                    Department = g.Key,
                    Issued = g.Count(),
                    Redeemed = g.Count(t => t.Status == "Redeemed"),
                    Pending = g.Count(t => t.Status == "Approved" || t.Status == "Pending"),
                    Variance = g.Count() - g.Count(t => t.Status == "Redeemed")
                })
                .ToList();

            // Create reconciliation record
            var reconciliation = new Reconciliation
            {
                ReconciliationDate = reconciliationDate,
                TotalTicketsIssued = totalIssued,
                TotalTicketsRedeemed = totalRedeemed,
                TotalTicketsPending = totalPending,
                Variance = totalIssued - totalRedeemed,
                Status = totalIssued == totalRedeemed ? "Completed" : "Pending",
                
                BreakfastIssued = breakfastIssued,
                BreakfastRedeemed = breakfastRedeemed,
                BreakfastVariance = breakfastIssued - breakfastRedeemed,
                
                LunchIssued = lunchIssued,
                LunchRedeemed = lunchRedeemed,
                LunchVariance = lunchIssued - lunchRedeemed,
                
                DinnerIssued = dinnerIssued,
                DinnerRedeemed = dinnerRedeemed,
                DinnerVariance = dinnerIssued - dinnerRedeemed,
                
                DepartmentBreakdown = JsonSerializer.Serialize(departmentBreakdown),
                Notes = "Auto-generated reconciliation"
            };

            _context.Reconciliations.Add(reconciliation);
            await _context.SaveChangesAsync();

            // If no variance, auto-complete
            if (reconciliation.Variance == 0)
            {
                await CompleteReconciliationAsync(reconciliation.Id, "No variance - auto completed");
            }

            return await GetReconciliationAsync(reconciliation.Id);
        }

        public async Task<ReconciliationSummaryDto> CompleteReconciliationAsync(int reconciliationId, string? notes = null)
        {
            var reconciliation = await _context.Reconciliations
                .Include(r => r.Reconciler)
                .FirstOrDefaultAsync(r => r.Id == reconciliationId);

            if (reconciliation == null)
                throw new InvalidOperationException("Reconciliation not found.");

            if (reconciliation.Status == "Completed")
                throw new InvalidOperationException("Reconciliation already completed.");

            reconciliation.Status = "Completed";
            reconciliation.ReconciledAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(notes))
                reconciliation.Notes = notes;

            await _context.SaveChangesAsync();

            // Update daily summary
            await UpdateDailySummaryAsync(reconciliation.ReconciliationDate);

            return await GetReconciliationAsync(reconciliationId);
        }

        public async Task<ReconciliationSummaryDto> GetReconciliationAsync(int id)
        {
            var reconciliation = await _context.Reconciliations
                .Include(r => r.Reconciler)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reconciliation == null)
                throw new InvalidOperationException("Reconciliation not found.");

            var departmentBreakdown = string.IsNullOrEmpty(reconciliation.DepartmentBreakdown)
                ? new List<DepartmentBreakdownDto>()
                : JsonSerializer.Deserialize<List<DepartmentBreakdownDto>>(reconciliation.DepartmentBreakdown) 
                    ?? new List<DepartmentBreakdownDto>();

            return new ReconciliationSummaryDto
            {
                Id = reconciliation.Id,
                ReconciliationDate = reconciliation.ReconciliationDate,
                TotalTicketsIssued = reconciliation.TotalTicketsIssued,
                TotalTicketsRedeemed = reconciliation.TotalTicketsRedeemed,
                TotalTicketsPending = reconciliation.TotalTicketsPending,
                Variance = reconciliation.Variance,
                Status = reconciliation.Status,
                ReconciledBy = reconciliation.Reconciler?.FullName,
                ReconciledAt = reconciliation.ReconciledAt,
                Notes = reconciliation.Notes,
                
                Breakfast = new MealTypeBreakdownDto
                {
                    Issued = reconciliation.BreakfastIssued,
                    Redeemed = reconciliation.BreakfastRedeemed,
                    Pending = reconciliation.BreakfastIssued - reconciliation.BreakfastRedeemed,
                    Variance = reconciliation.BreakfastVariance
                },
                Lunch = new MealTypeBreakdownDto
                {
                    Issued = reconciliation.LunchIssued,
                    Redeemed = reconciliation.LunchRedeemed,
                    Pending = reconciliation.LunchIssued - reconciliation.LunchRedeemed,
                    Variance = reconciliation.LunchVariance
                },
                Dinner = new MealTypeBreakdownDto
                {
                    Issued = reconciliation.DinnerIssued,
                    Redeemed = reconciliation.DinnerRedeemed,
                    Pending = reconciliation.DinnerIssued - reconciliation.DinnerRedeemed,
                    Variance = reconciliation.DinnerVariance
                },
                DepartmentBreakdown = departmentBreakdown
            };
        }

        public async Task<List<ReconciliationSummaryDto>> GetReconciliationsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reconciliations
                .Include(r => r.Reconciler)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.ReconciliationDate >= startDate.Value.Date);
            
            if (endDate.HasValue)
                query = query.Where(r => r.ReconciliationDate <= endDate.Value.Date);

            var reconciliations = await query
                .OrderByDescending(r => r.ReconciliationDate)
                .ToListAsync();

            var result = new List<ReconciliationSummaryDto>();
            foreach (var r in reconciliations)
            {
                result.Add(await GetReconciliationAsync(r.Id));
            }

            return result;
        }

        public async Task<List<VarianceDetailDto>> GetVarianceDetailsAsync(int reconciliationId)
        {
            var reconciliation = await _context.Reconciliations
                .FindAsync(reconciliationId);

            if (reconciliation == null)
                throw new InvalidOperationException("Reconciliation not found.");

            // Load all tickets for that date into memory
            var tickets = await _context.MealTickets
                .Include(t => t.Employee)
                .Where(t => t.TicketDate.Date == reconciliation.ReconciliationDate.Date)
                .ToListAsync();

            var variances = new List<VarianceDetailDto>();

            // 1. Missing tickets (approved/pending but not redeemed)
            var missingTickets = tickets
                .Where(t => t.Status == "Approved" || t.Status == "Pending")
                .Select(t => new VarianceDetailDto
                {
                    TicketNumber = t.TicketNumber,
                    EmployeeName = t.EmployeeName ?? "Unknown",
                    Department = t.Department ?? "Unknown",
                    MealType = t.MealType,
                    TicketDate = t.TicketDate,
                    Status = t.Status,
                    VarianceType = "Missing"
                });
            variances.AddRange(missingTickets);

            // 2. Duplicate redemptions – load redemption logs into memory first
            var allRedemptionLogs = await _context.RedemptionLogs.ToListAsync();

            var duplicateRedemptions = allRedemptionLogs
                .GroupBy(l => l.TicketNumber)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .Select(l =>
                {
                    var ticket = tickets.FirstOrDefault(t => t.TicketNumber == l.TicketNumber);
                    return new VarianceDetailDto
                    {
                        TicketNumber = l.TicketNumber,
                        EmployeeName = ticket?.EmployeeName ?? "Unknown",  // Safe here (in-memory)
                        Department = ticket?.Department ?? "Unknown",
                        MealType = ticket?.MealType ?? "Unknown",
                        TicketDate = l.RedemptionTime,
                        Status = "Redeemed",
                        VarianceType = "Duplicate"
                    };
                })
                .ToList();

            variances.AddRange(duplicateRedemptions);

            return variances;
        }

        public async Task<bool> AutoReconcileAsync(int reconciliationId)
        {
            var reconciliation = await _context.Reconciliations
                .FindAsync(reconciliationId);

            if (reconciliation == null)
                return false;

            if (reconciliation.Variance != 0)
                return false;

            await CompleteReconciliationAsync(reconciliationId, "Auto-reconciled - no variance");
            return true;
        }

        public async Task<ReconciliationSummaryDto> ReconcileDateAsync(DateTime date)
        {
            var reconciliationDate = date.Date;
            
            // Check if reconciliation exists
            var existing = await _context.Reconciliations
                .FirstOrDefaultAsync(r => r.ReconciliationDate.Date == reconciliationDate);

            if (existing != null)
                return await GetReconciliationAsync(existing.Id);

            // Create new reconciliation
            return await CreateReconciliationAsync(reconciliationDate);
        }

        private async Task UpdateDailySummaryAsync(DateTime date)
        {
            var summary = await _context.DailySummaries
                .FirstOrDefaultAsync(s => s.SummaryDate.Date == date.Date);

            if (summary == null)
            {
                summary = new DailySummary
                {
                    SummaryDate = date.Date
                };
                _context.DailySummaries.Add(summary);
            }

            // Get ticket data
            var tickets = await _context.MealTickets
                .Where(t => t.TicketDate.Date == date.Date)
                .ToListAsync();

            // Set default meal prices (could be configurable)
            const decimal breakfastPrice = 5.00m;
            const decimal lunchPrice = 10.00m;
            const decimal dinnerPrice = 8.00m;

            summary.BreakfastCount = tickets.Count(t => t.MealType == "Breakfast" && t.Status == "Redeemed");
            summary.LunchCount = tickets.Count(t => t.MealType == "Lunch" && t.Status == "Redeemed");
            summary.DinnerCount = tickets.Count(t => t.MealType == "Dinner" && t.Status == "Redeemed");
            
            summary.TotalMealsServed = summary.BreakfastCount + summary.LunchCount + summary.DinnerCount;
            
            summary.BreakfastRevenue = summary.BreakfastCount * breakfastPrice;
            summary.LunchRevenue = summary.LunchCount * lunchPrice;
            summary.DinnerRevenue = summary.DinnerCount * dinnerPrice;
            summary.TotalRevenue = summary.BreakfastRevenue + summary.LunchRevenue + summary.DinnerRevenue;

            summary.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
