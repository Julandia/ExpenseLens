using BackendService.Repositories.Models;

namespace BackendService.Repositories;

public class InMemoryExpenseRepository : IExpenseRepository
{
    private readonly List<Receipt> _receipts = new();

    public Task SaveReceiptsAsync(IEnumerable<Receipt> receipts, CancellationToken cancellationToken = default)
    {
        _receipts.AddRange(receipts);
        return Task.CompletedTask;
    }

    public Task<Receipt> GetReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Receipt>> GetReceiptsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_receipts.AsEnumerable());
    }
}
