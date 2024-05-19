namespace BackendService.Features.v1.ScanReceipt;

public record ScanReceiptResponse(string MerchantName, DateTime Date, double TotalPrice, int ItemCount);
