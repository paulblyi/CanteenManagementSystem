using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CanteenApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public EmployeeController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost("request-ticket")]
        public async Task<IActionResult> RequestTicket([FromBody] TicketRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var ticket = await _ticketService.RequestTicketAsync(userId, request);
                return Ok(ticket);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = GetCurrentUserId();
            var tickets = await _ticketService.GetEmployeeTicketsAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("ticket/{ticketNumber}")]
        public async Task<IActionResult> GetTicket(string ticketNumber)
        {
            var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);
            if (ticket == null)
                return NotFound(new { message = "Ticket not found." });

            // Check if the ticket belongs to the current user
            var userId = GetCurrentUserId();
            if (ticket.EmployeeId != userId)
                return Forbid();

            return Ok(ticket);
        }

        [HttpDelete("cancel-ticket/{ticketId}")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            var userId = GetCurrentUserId();
            var success = await _ticketService.CancelTicketAsync(ticketId, userId);
            if (!success)
                return BadRequest(new { message = "Unable to cancel ticket." });

            return Ok(new { message = "Ticket cancelled successfully." });
        }
    }
}
