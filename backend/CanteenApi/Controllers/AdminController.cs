using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CanteenApi.DTOs;
using CanteenApi.Services;
using Microsoft.EntityFrameworkCore;
using CanteenApi.Data;

namespace CanteenApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;        // ← Added
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
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

        [Authorize(Roles = "Admin,HumanCapital")]
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Users
                .Where(u => u.Role == "Employee" && u.IsActive)
                .Include(u => u.Department)
                .OrderBy(u => u.FullName)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(employees);
        }
    }
}