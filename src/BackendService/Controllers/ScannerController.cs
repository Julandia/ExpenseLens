using BackendService.Features.v1.ScanReceipt;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers;

[Route("api/v{apiVersion:apiVersion}/[controller]")]
[ApiController]
public class ScannerController : ControllerBase
{
    /// <summary>
    /// Scan a receipt and return the overview information.
    /// Details are saved in the database.
    /// </summary>
    [HttpPost("receipts")]
    [ProducesResponseType(typeof(ScanReceiptResponse), 200)]
    public async Task<IActionResult> ScanReceipt([FromBody] ScanReceiptsRequest request,
        [FromServices] ScanReceiptHandler handler,
        CancellationToken cancellationToken = default)
    {
        var response = await handler.Handle(request, cancellationToken);
        return response.Match<IActionResult>(
            _ => NotFound(),
            res => Ok(res.Value));
    }

    [HttpGet("receipts/{receiptId}")]
    [ProducesResponseType(typeof(GetReceiptResponse), 200)]
    public async Task<IActionResult> GetReceipt([FromRoute] string receiptId,
        [FromServices] GetReceiptHandler handler,
        CancellationToken cancellationToken = default)
    {
        var response = await handler.Handle(new GetReceiptCommand(receiptId), cancellationToken);
        return response.Match<IActionResult>(
            _ => NotFound(),
            res => Ok(res.Value));
    }
}
