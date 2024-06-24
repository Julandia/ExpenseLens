using BackendService.Repositories.Models;

namespace BackendService.Repositories.Database;

public interface IExpenseRepository
{
    Task SaveReceiptsAsync(IEnumerable<Receipt> receipts, CancellationToken cancellationToken = default);
    Task SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken = default);
    Task<Receipt?> GetReceiptAsync(string receiptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Receipt>> GetReceiptsAsync(CancellationToken cancellationToken = default);
}
