using CanteenApi.DTOs;

namespace CanteenApi.Services
{
    public interface ITicketService
    {
        Task<TicketResponseDto> RequestTicketAsync(int employeeId, TicketRequestDto request);
        Task<List<TicketResponseDto>> GetEmployeeTicketsAsync(int employeeId);
        Task<List<TicketResponseDto>> GetPendingTicketsAsync(string? department = null);
        Task<bool> ApproveTicketAsync(int approverId, TicketApprovalDto approval);
        Task<TicketResponseDto?> RedeemTicketAsync(int chefId, TicketRedemptionDto redemption);
        Task<TicketResponseDto?> GetTicketByNumberAsync(string ticketNumber);
        Task<bool> CancelTicketAsync(int ticketId, int userId);
    }
}