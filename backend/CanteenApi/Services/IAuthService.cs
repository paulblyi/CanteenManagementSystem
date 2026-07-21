using CanteenApi.DTOs;
using CanteenApi.Models;

namespace CanteenApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UserExistsAsync(string username);

        // Additional methods for user management
        Task<List<UserListDto>> GetUsersAsync();
        Task<UserListDto> CreateUserAsync(CreateUserRequest request);
        Task<bool> ToggleUserActiveAsync(int userId);
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);
        Task<bool> ResetUserPasswordAsync(int userId, string newPassword);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    }
}
