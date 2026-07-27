using CanteenApi.Data;
using CanteenApi.DTOs;
using CanteenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync(bool includeInactive = false)
        {
            var query = _context.Departments.AsQueryable();
            if (!includeInactive)
                query = query.Where(d => d.IsActive);

            var departments = await query
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UserCount = d.Users.Count
                })
                .ToListAsync();

            return departments;
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            var dept = await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dept == null) return null;

            return new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name,
                Description = dept.Description,
                IsActive = dept.IsActive,
                CreatedAt = dept.CreatedAt,
                UserCount = dept.Users.Count
            };
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            // Check duplicate name
            if (await _context.Departments.AnyAsync(d => d.Name == request.Name))
                throw new InvalidOperationException("Department with this name already exists.");

            var dept = new Department
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            return await GetDepartmentByIdAsync(dept.Id) ?? throw new Exception("Failed to create department.");
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return null;

            // Check duplicate name (excluding self)
            if (await _context.Departments.AnyAsync(d => d.Name == request.Name && d.Id != id))
                throw new InvalidOperationException("Another department with this name already exists.");

            dept.Name = request.Name;
            dept.Description = request.Description;
            dept.IsActive = request.IsActive;
            dept.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetDepartmentByIdAsync(id);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var dept = await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dept == null) return false;

            // Check if any users are assigned
            if (dept.Users.Any())
                throw new InvalidOperationException("Cannot delete department with assigned users. Reassign users first.");

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleDepartmentActiveAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return false;

            dept.IsActive = !dept.IsActive;
            dept.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}