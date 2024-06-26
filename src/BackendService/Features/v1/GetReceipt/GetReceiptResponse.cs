using BackendService.Repositories.Models;

namespace BackendService.Features.v1.ScanReceipt;

public class GetReceiptResponse
{
    public Receipt? Receipt { get; init; }
}
