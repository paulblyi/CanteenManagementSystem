using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CanteenApi.DTOs;
using CanteenApi.Services;
using CanteenApi.Models;

[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var departments = await _departmentService.GetAllDepartmentsAsync(includeInactive);
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _departmentService.GetDepartmentByIdAsync(id);
        if (dept == null) return NotFound();
        return Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var dept = await _departmentService.CreateDepartmentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = dept.Id }, dept);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequest request)
    {
        try
        {
            var dept = await _departmentService.UpdateDepartmentAsync(id, request);
            if (dept == null) return NotFound();
            return Ok(dept);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _departmentService.ToggleDepartmentActiveAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Department active status toggled." });
    }
}