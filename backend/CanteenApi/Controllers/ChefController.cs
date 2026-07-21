using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CanteenApi.Controllers
{
    [Authorize(Roles = "Chef,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChefController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public ChefController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost("redeem-ticket")]
        public async Task<IActionResult> RedeemTicket([FromBody] TicketRedemptionDto redemption)
        {
            var userId = GetCurrentUserId();
            var ticket = await _ticketService.RedeemTicketAsync(userId, redemption);

            if (ticket == null)
                return BadRequest(new { message = "Invalid ticket or ticket not approved." });

            return Ok(new { message = "Ticket redeemed successfully.", ticket });
        }

        [HttpGet("validate-ticket/{ticketNumber}")]
        public async Task<IActionResult> ValidateTicket(string ticketNumber)
        {
            var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);

            if (ticket == null)
                return NotFound(new { message = "Ticket not found.", isValid = false });

            if (ticket.Status != "Approved")
                return Ok(new { isValid = false, message = $"Ticket is {ticket.Status.ToLower()}.", ticket });

            if (ticket.TicketDate.Date != DateTime.UtcNow.Date)
                return Ok(new { isValid = false, message = "Ticket is not valid for today.", ticket });

            return Ok(new { isValid = true, message = "Ticket is valid.", ticket });
        }

        // Additional methods for the ChefController can be added here
        [HttpGet("recent-redemptions")]
        /*         public async Task<IActionResult> GetRecentRedemptions([FromQuery] int count = 10)
                {
                    var redemptions = await _context.RedemptionLogs
                        .Include(r => r.Employee)
                        .OrderByDescending(r => r.RedemptionTime)
                        .Take(count)
                        .Select(r => new {
                            TicketNumber = r.TicketNumber,
                            EmployeeName = r.Employee != null ? r.Employee.FullName : "Unknown",
                            RedeemedAt = r.RedemptionTime
                        })
                        .ToListAsync();

                    return Ok(redemptions);
                } */
        public async Task<IActionResult> GetRecentRedemptions([FromQuery] int count = 10)
        {
            var redemptions = await _ticketService.GetRecentRedemptionsAsync(count);
            return Ok(redemptions);
        }
    }
}
