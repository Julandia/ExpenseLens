using System.Text.Json.Serialization;

namespace BackendService.Repositories.Models;

public class Receipt
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Category { get; init; } = DocumentCategory.Receipt.ToString();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string MerchantName { get; init; }
    public DateTime Date { get; init; }
    public double TotalPrice { get; init; }

    public IReadOnlyList<ReceiptItem> Items { get; init; } = new List<ReceiptItem>();
}
