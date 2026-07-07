using CanteenApi.DTOs;

namespace CanteenApi.Services
{
    public interface IReconciliationService
    {
        Task<ReconciliationSummaryDto> CreateReconciliationAsync(DateTime date);
        Task<ReconciliationSummaryDto> CompleteReconciliationAsync(int reconciliationId, string? notes = null);
        Task<ReconciliationSummaryDto> GetReconciliationAsync(int id);
        Task<List<ReconciliationSummaryDto>> GetReconciliationsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<VarianceDetailDto>> GetVarianceDetailsAsync(int reconciliationId);
        Task<bool> AutoReconcileAsync(int reconciliationId);
        Task<ReconciliationSummaryDto> ReconcileDateAsync(DateTime date);
    }
}
