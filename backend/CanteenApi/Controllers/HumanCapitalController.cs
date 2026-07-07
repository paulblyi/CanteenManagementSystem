using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CanteenApi.Controllers
{
    [Authorize(Roles = "HumanCapital,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class HumanCapitalController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IBatchService _batchService;

        public HumanCapitalController(ITicketService ticketService, IBatchService batchService)
        {
            _ticketService = ticketService;
            _batchService = batchService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet("pending-tickets")]
        public async Task<IActionResult> GetPendingTickets([FromQuery] string? department = null)
        {
            var tickets = await _ticketService.GetPendingTicketsAsync(department);
            return Ok(tickets);
        }

        [HttpPost("approve-ticket")]
        public async Task<IActionResult> ApproveTicket([FromBody] TicketApprovalDto approval)
        {
            var userId = GetCurrentUserId();
            var success = await _ticketService.ApproveTicketAsync(userId, approval);
            if (!success)
                return BadRequest(new { message = "Unable to approve ticket." });

            return Ok(new { message = $"Ticket {approval.Status} successfully." });
        }

        [HttpPost("create-batch")]
        public async Task<IActionResult> CreateBatch([FromBody] BatchCreateDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var batch = await _batchService.CreateBatchAsync(userId, request);
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches([FromQuery] DateTime? date = null)
        {
            var batches = await _batchService.GetBatchesAsync(date);
            return Ok(batches);
        }

        [HttpPut("cancel-batch/{batchId}")]
        public async Task<IActionResult> CancelBatch(int batchId)
        {
            var success = await _batchService.CancelBatchAsync(batchId);
            if (!success)
                return BadRequest(new { message = "Unable to cancel batch." });

            return Ok(new { message = "Batch cancelled successfully." });
        }
    }
}
