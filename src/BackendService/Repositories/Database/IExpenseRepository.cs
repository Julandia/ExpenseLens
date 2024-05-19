using BackendService.Repositories.Models;

namespace BackendService.Repositories;

public interface IExpenseRepository
{
    Task SaveReceiptsAsync(IEnumerable<Receipt> receipts, CancellationToken cancellationToken = default);
    Task<Receipt> GetReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Receipt>> GetReceiptsAsync(CancellationToken cancellationToken = default);
}
