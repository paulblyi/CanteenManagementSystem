using CanteenApi.DTOs;

namespace CanteenApi.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllDepartmentsAsync(bool includeInactive = false);
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request);
        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<bool> ToggleDepartmentActiveAsync(int id);
    }
}