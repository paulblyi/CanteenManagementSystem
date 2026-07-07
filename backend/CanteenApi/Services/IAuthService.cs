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
    }
}
