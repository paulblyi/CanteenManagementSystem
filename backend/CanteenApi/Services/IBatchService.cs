using CanteenApi.DTOs;

namespace CanteenApi.Services
{
    public interface IBatchService
    {
        Task<BatchResponseDto> CreateBatchAsync(int createdBy, BatchCreateDto request);
        Task<List<BatchResponseDto>> GetBatchesAsync(DateTime? date = null);
        Task<BatchResponseDto?> GetBatchByIdAsync(int batchId);
        Task<bool> CancelBatchAsync(int batchId);
        Task<bool> ProcessBatchAsync(int batchId);
    }
}
