using BackendService.Configuration;
using BackendService.Features.Infrastructure;
using BackendService.Repositories.Database;
using BackendService.Repositories.FileStorage;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace BackendService.Features.v1.ScanReceipt;

public class GetReceiptHandler : IRequestHandler<GetReceiptCommand, OneOf<NotFound, Success<GetReceiptResponse>>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<GetReceiptHandler> _logger;

    public GetReceiptHandler(IExpenseRepository expenseRepository,
        ILogger<GetReceiptHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _logger = logger;
    }

    public async Task<OneOf<NotFound, Success<GetReceiptResponse>>> Handle(GetReceiptCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Retrieving receipt {command.ReceiptId}");

        var receipt = await _expenseRepository.GetReceiptAsync(command.ReceiptId, cancellationToken);

        _logger.LogInformation("{Count} receipt is retrieved", receipt == null ? 0 : 1);
        return new Success<GetReceiptResponse>(new GetReceiptResponse { Receipt = receipt });
    }
}
