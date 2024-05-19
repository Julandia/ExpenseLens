namespace BackendService.Repositories.Models;

public record Receipt(string MerchantName, DateTime Date, double TotalPrice, IReadOnlyList<ReceiptItem> Items);
