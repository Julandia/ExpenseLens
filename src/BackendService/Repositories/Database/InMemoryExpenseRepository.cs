using BackendService.Repositories.Models;

namespace BackendService.Repositories.Database;

public class InMemoryExpenseRepository : IExpenseRepository
{
    private readonly List<Receipt> _receipts = new();

    public Task SaveReceiptsAsync(IEnumerable<Receipt> receipts, CancellationToken cancellationToken = default)
    {
        _receipts.AddRange(receipts);
        return Task.CompletedTask;
    }

    public Task SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Receipt?> GetReceiptAsync(string receiptId, CancellationToken cancellationToken = default)
    {
        var receipt = _receipts.FirstOrDefault(receipt => receipt.Id == receiptId);
        return Task.FromResult(receipt);
    }

    public Task<IEnumerable<Receipt>> GetReceiptsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_receipts.AsEnumerable());
    }
}
