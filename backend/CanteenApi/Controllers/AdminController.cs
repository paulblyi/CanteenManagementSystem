using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CanteenApi.DTOs;
using CanteenApi.Services;

namespace CanteenApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _authService.CreateUserAsync(request);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var result = await _authService.ToggleUserActiveAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "User active status toggled." });
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
        {
            var result = await _authService.UpdateUserRoleAsync(id, request.Role);
            if (!result) return NotFound();
            return Ok(new { message = "User role updated." });
        }

        [HttpPut("users/{id}/reset-password")]
        public async Task<IActionResult> ResetUserPassword(int id, [FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetUserPasswordAsync(id, request.NewPassword);
            if (!result) return NotFound();
            return Ok(new { message = "Password reset successfully." });
        }
    }
}