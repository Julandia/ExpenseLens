namespace BackendService.Repositories.Models;

public record ReceiptItem(string Description, double Price, int Quantity, double TotalPrice);
