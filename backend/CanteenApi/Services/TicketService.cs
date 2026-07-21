using CanteenApi.Data;
using CanteenApi.DTOs;
using CanteenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TicketResponseDto> RequestTicketAsync(int employeeId, TicketRequestDto request)
        {
            // Check if employee already has a ticket for this date and meal
            var existingTicket = await _context.MealTickets
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId
                    && t.TicketDate.Date == request.TicketDate.Date
                    && t.MealType == request.MealType
                    && t.Status != "Cancelled");

            if (existingTicket != null)
                throw new InvalidOperationException("You already have a ticket for this meal on this date.");

            var employee = await _context.Users.FindAsync(employeeId);
            if (employee == null)
                throw new InvalidOperationException("Employee not found.");

            var ticket = new MealTicket
            {
                TicketNumber = GenerateTicketNumber(),
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                Department = employee.Department,
                TicketDate = request.TicketDate,
                MealType = request.MealType,
                Status = "Pending",
                Notes = request.Notes,
                QRCode = GenerateQRCode()
            };

            _context.MealTickets.Add(ticket);
            await _context.SaveChangesAsync();

            return MapToResponse(ticket);
        }

        public async Task<List<TicketResponseDto>> GetEmployeeTicketsAsync(int employeeId)
        {
            var tickets = await _context.MealTickets
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.TicketDate)
                .ToListAsync();

            return tickets.Select(MapToResponse).ToList();
        }

        public async Task<List<TicketResponseDto>> GetPendingTicketsAsync(string? department = null)
        {
            var query = _context.MealTickets
                .Where(t => t.Status == "Pending")
                .Include(t => t.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(department))
                query = query.Where(t => t.Department == department);

            var tickets = await query
                .OrderBy(t => t.TicketDate)
                .ToListAsync();

            return tickets.Select(MapToResponse).ToList();
        }

        public async Task<bool> ApproveTicketAsync(int approverId, TicketApprovalDto approval)
        {
            var ticket = await _context.MealTickets.FindAsync(approval.TicketId);
            if (ticket == null || ticket.Status != "Pending")
                return false;

            ticket.Status = approval.Status;
            ticket.ApprovedBy = approverId;
            ticket.ApprovedAt = DateTime.UtcNow;
            ticket.Notes = approval.Notes ?? ticket.Notes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TicketResponseDto?> RedeemTicketAsync(int chefId, TicketRedemptionDto redemption)
        {
            var ticket = await _context.MealTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == redemption.TicketNumber);

            if (ticket == null || ticket.Status != "Approved")
                return null;

            // Check if ticket is for today
            if (ticket.TicketDate.Date != DateTime.UtcNow.Date)
                return null;

            ticket.Status = "Redeemed";
            ticket.RedeemedAt = DateTime.UtcNow;

            // Log redemption
            var log = new RedemptionLog
            {
                TicketNumber = ticket.TicketNumber,
                EmployeeId = ticket.EmployeeId,
                ChefId = chefId,
                VerificationMethod = redemption.VerificationMethod,
                Status = "Success"
            };

            _context.RedemptionLogs.Add(log);
            await _context.SaveChangesAsync();

            return MapToResponse(ticket);
        }

        public async Task<TicketResponseDto?> GetTicketByNumberAsync(string ticketNumber)
        {
            var ticket = await _context.MealTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

            return ticket == null ? null : MapToResponse(ticket);
        }

        public async Task<bool> CancelTicketAsync(int ticketId, int userId)
        {
            var ticket = await _context.MealTickets.FindAsync(ticketId);
            if (ticket == null || ticket.Status == "Redeemed")
                return false;

            ticket.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateTicketNumber()
        {
            return $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        private string GenerateQRCode()
        {
            // Simplified QR code generation - in production use a QR library
            return $"QR-{Guid.NewGuid().ToString().Substring(0, 10)}";
        }

        private TicketResponseDto MapToResponse(MealTicket ticket)
        {
            return new TicketResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.EmployeeName ?? "Unknown",
                Department = ticket.Department ?? "Unknown",
                TicketDate = ticket.TicketDate,
                MealType = ticket.MealType,
                Status = ticket.Status,
                RequestedAt = ticket.RequestedAt,
                ApprovedAt = ticket.ApprovedAt,
                RedeemedAt = ticket.RedeemedAt,
                QRCode = ticket.QRCode
            };
        }


        public async Task<List<RecentRedemptionDto>> GetRecentRedemptionsAsync(int count = 10)
        {
            var redemptions = await _context.RedemptionLogs
                .Include(r => r.Employee)
                .OrderByDescending(r => r.RedemptionTime)
                .Take(count)
                .Select(r => new RecentRedemptionDto
                {
                    TicketNumber = r.TicketNumber,
                    EmployeeName = r.Employee != null ? r.Employee.FullName : "Unknown",
                    RedeemedAt = r.RedemptionTime
                })
                .ToListAsync();

            return redemptions;
        }
    }
}
