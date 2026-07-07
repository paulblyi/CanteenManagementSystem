using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenApi.Controllers
{
    [Authorize(Roles = "Admin,Finance")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReconciliationController : ControllerBase
    {
        private readonly IReconciliationService _reconciliationService;
        private readonly ILogger<ReconciliationController> _logger;

        public ReconciliationController(
            IReconciliationService reconciliationService,
            ILogger<ReconciliationController> logger)
        {
            _reconciliationService = reconciliationService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReconciliation([FromBody] ReconciliationCreateDto request)
        {
            try
            {
                var result = await _reconciliationService.CreateReconciliationAsync(request.ReconciliationDate);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reconciliation");
                return StatusCode(500, new { message = "An error occurred creating reconciliation." });
            }
        }

        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteReconciliation(int id, [FromBody] ReconciliationCompleteDto? request = null)
        {
            try
            {
                var result = await _reconciliationService.CompleteReconciliationAsync(id, request?.Notes);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing reconciliation");
                return StatusCode(500, new { message = "An error occurred completing reconciliation." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReconciliation(int id)
        {
            try
            {
                var result = await _reconciliationService.GetReconciliationAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reconciliation");
                return StatusCode(500, new { message = "An error occurred retrieving reconciliation." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReconciliations([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var results = await _reconciliationService.GetReconciliationsAsync(startDate, endDate);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reconciliations");
                return StatusCode(500, new { message = "An error occurred retrieving reconciliations." });
            }
        }

        [HttpGet("{id}/variances")]
        public async Task<IActionResult> GetVarianceDetails(int id)
        {
            try
            {
                var results = await _reconciliationService.GetVarianceDetailsAsync(id);
                return Ok(results);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting variance details");
                return StatusCode(500, new { message = "An error occurred retrieving variance details." });
            }
        }

        [HttpPost("reconcile-date")]
        public async Task<IActionResult> ReconcileDate([FromBody] ReconciliationCreateDto request)
        {
            try
            {
                var result = await _reconciliationService.ReconcileDateAsync(request.ReconciliationDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reconciling date");
                return StatusCode(500, new { message = "An error occurred reconciling date." });
            }
        }

        [HttpPost("{id}/auto-reconcile")]
        public async Task<IActionResult> AutoReconcile(int id)
        {
            try
            {
                var success = await _reconciliationService.AutoReconcileAsync(id);
                if (!success)
                    return BadRequest(new { message = "Cannot auto-reconcile - variance exists." });

                var result = await _reconciliationService.GetReconciliationAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-reconciling");
                return StatusCode(500, new { message = "An error occurred during auto-reconciliation." });
            }
        }
    }
}
