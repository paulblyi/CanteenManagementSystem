using CanteenApi.Data;
using CanteenApi.DTOs;
using CanteenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenApi.Services
{
    public class BatchService : IBatchService
    {
        private readonly AppDbContext _context;
        private readonly ITicketService _ticketService;

        public BatchService(AppDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }

        public async Task<BatchResponseDto> CreateBatchAsync(int createdBy, BatchCreateDto request)
        {
            var employees = await _context.Users
                .Where(u => request.EmployeeIds.Contains(u.Id) && u.Role == "Employee")
                .ToListAsync();

            if (!employees.Any())
                throw new InvalidOperationException("No valid employees selected.");

            // Create batch
            var batch = new Batch
            {
                BatchNumber = GenerateBatchNumber(),
                CreatedBy = createdBy,
                Department = request.Department ?? employees.FirstOrDefault()?.Department?.Name,
                TicketDate = request.TicketDate,
                MealType = request.MealType,
                TotalTickets = employees.Count,
                Status = "Active"
            };

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            // Create tickets for each employee
            foreach (var employee in employees)
            {
                var ticketRequest = new TicketRequestDto
                {
                    TicketDate = request.TicketDate,
                    MealType = request.MealType,
                    Notes = $"Batch {batch.BatchNumber}"
                };

                var ticket = await _ticketService.RequestTicketAsync(employee.Id, ticketRequest);

                // Update ticket with batch info
                var dbTicket = await _context.MealTickets.FindAsync(ticket.Id);
                if (dbTicket != null)
                {
                    dbTicket.BatchId = batch.Id;
                    // Auto-approve batch tickets
                    dbTicket.Status = "Approved";
                    dbTicket.ApprovedBy = createdBy;
                    dbTicket.ApprovedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            // Reload batch with tickets
            return await GetBatchByIdAsync(batch.Id) ?? throw new InvalidOperationException("Failed to create batch.");
        }

        public async Task<List<BatchResponseDto>> GetBatchesAsync(DateTime? date = null)
        {
            var query = _context.Batches
                .Include(b => b.Tickets)
                .Include(b => b.Creator)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(b => b.TicketDate.Date == date.Value.Date);

            var batches = await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return batches.Select(MapToResponse).ToList();
        }

        public async Task<BatchResponseDto?> GetBatchByIdAsync(int batchId)
        {
            var batch = await _context.Batches
                .Include(b => b.Tickets)
                .Include(b => b.Creator)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            return batch == null ? null : MapToResponse(batch);
        }

        public async Task<bool> CancelBatchAsync(int batchId)
        {
            var batch = await _context.Batches
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            if (batch == null || batch.Status != "Active")
                return false;

            batch.Status = "Cancelled";
            foreach (var ticket in batch.Tickets)
            {
                if (ticket.Status == "Pending" || ticket.Status == "Approved")
                    ticket.Status = "Cancelled";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProcessBatchAsync(int batchId)
        {
            var batch = await _context.Batches
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            if (batch == null || batch.Status != "Active")
                return false;

            // Check if all tickets are redeemed
            bool allRedeemed = batch.Tickets.All(t => t.Status == "Redeemed");
            if (allRedeemed)
            {
                batch.Status = "Completed";
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private string GenerateBatchNumber()
        {
            return $"BATCH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        private BatchResponseDto MapToResponse(Batch batch)
        {
            return new BatchResponseDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                Department = batch.Department ?? "All Departments",
                TicketDate = batch.TicketDate,
                MealType = batch.MealType,
                TotalTickets = batch.TotalTickets,
                Status = batch.Status,
                CreatedAt = batch.CreatedAt,
                Tickets = batch.Tickets.Select(t => new TicketResponseDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    EmployeeName = t.EmployeeName ?? "Unknown",
                    Department = t.Department ?? "Unknown",
                    MealType = t.MealType,
                    Status = t.Status,
                    QRCode = t.QRCode
                }).ToList()
            };
        }
    }
}
