using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using BackendService.Configuration;
using BackendService.Features.Infrastructure;
using BackendService.Repositories.Database;
using BackendService.Repositories.FileStorage;
using BackendService.Repositories.Models;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace BackendService.Features.v1.ScanReceipt;

public class ScanReceiptHandler : IRequestHandler<ScanReceiptsRequest, OneOf<NotFound, Success<IEnumerable<ScanReceiptResponse>>>>
{
    private readonly AzureAiVisionConfig _config;
    private readonly BlobStorageConfig _blobStorageConfig;
    private readonly IFileStorage _fileStorage;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<ScanReceiptHandler> _logger;

    public ScanReceiptHandler(IOptions<AzureAiVisionConfig> options,
        IOptions<BlobStorageConfig> blobStorageOptions,
        IFileStorage fileStorage,
        IExpenseRepository expenseRepository,
        ILogger<ScanReceiptHandler> logger)
    {
        _config = options.Value;
        _blobStorageConfig = blobStorageOptions.Value;
        _fileStorage = fileStorage;
        _expenseRepository = expenseRepository;
        _logger = logger;
    }

    public async Task<OneOf<NotFound, Success<IEnumerable<ScanReceiptResponse>>>> Handle(ScanReceiptsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing receipt {request.FileName}");

        if (!await _fileStorage.CheckIfReceiptFileExistsAsync(request.FileName, cancellationToken))
        {
            _logger.LogWarning("Receipt file {FileName} does not exist", request.FileName);
            return new NotFound();
        }

        var receipts = await ProcessReceipt(request.FileName);
        if (receipts.Count > 0)
        {
            await _expenseRepository.SaveReceiptAsync(receipts.First(), cancellationToken);
        }

        _logger.LogInformation("{Count} receipts are processed", receipts.Count);
        return new Success<IEnumerable<ScanReceiptResponse>>(receipts.Select(receipt => new ScanReceiptResponse(receipt.MerchantName, receipt.Date, receipt.TotalPrice, receipt.Items.Count)));
    }

    private async Task<List<Receipt>> ProcessReceipt(string fileName)
    {
        var credential = new AzureKeyCredential(_config.ApiKey);
        var client = new DocumentAnalysisClient(new Uri(_config.Endpoint), credential);

        // sample document document
        //Uri receiptUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/receipt.png");
        var receiptUri = new Uri($"https://{_blobStorageConfig.StorageAccountName}.blob.core.windows.net/{_blobStorageConfig.ReceiptsContainerName}/{fileName}");

        var operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-receipt", receiptUri);

        var receipts = new List<Receipt>();

        foreach (var document in operation.Value.Documents)
        {
            var merchantName = document.Fields.TryGetValue("MerchantName", out DocumentField? merchantNameField) && merchantNameField.FieldType == DocumentFieldType.String
                ? merchantNameField.Value.AsString()
                : string.Empty;
            _logger.LogInformation("Merchant Name: '{MerchantName}', with confidence {Confidence}", merchantName, merchantNameField?.Confidence);

            DateTimeOffset transactionDate = document.Fields.TryGetValue("TransactionDate", out DocumentField? transactionDateField) && transactionDateField.FieldType == DocumentFieldType.Date
                ? transactionDateField.Value.AsDate()
                : DateTimeOffset.MinValue;
            _logger.LogInformation("Transaction Date: '{TransactionDate}', with confidence {Confidence}", transactionDate, transactionDateField?.Confidence);

            var items = new List<ReceiptItem>();
            if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            var itemFields = itemField.Value.AsDictionary();
                            var itemDescription = itemFields.TryGetValue("Description", out DocumentField? itemDescriptionField) && itemDescriptionField.FieldType == DocumentFieldType.String
                                ? itemDescriptionField.Value.AsString()
                                : string.Empty;
                            var itemTotalPrice = itemFields.TryGetValue("TotalPrice", out DocumentField? itemTotalPriceField) && itemTotalPriceField.FieldType == DocumentFieldType.Double
                                ? itemTotalPriceField.Value.AsDouble()
                                : 0;
                            items.Add(new ReceiptItem(itemDescription, 0, 1, itemTotalPrice));
                            _logger.LogInformation("Item Description: '{Description}', with confidence {Confidence}; Total price: '{TotalPrice}', with confidence {Confidence}",
                                itemDescription, itemDescriptionField?.Confidence ?? 0, itemTotalPrice, itemTotalPriceField?.Confidence ?? 0);
                        }
                    }
                }
            }

            var total = document.Fields.TryGetValue("Total", out DocumentField? totalField) && totalField.FieldType == DocumentFieldType.Double
                ? totalField.Value.AsDouble()
                : 0;
            _logger.LogInformation("Total: '{Total}', with confidence '{Confidence}'", total, totalField?.Confidence);

            receipts.Add(new Receipt {
                    MerchantName = merchantName,
                    Date = transactionDate.DateTime,
                    TotalPrice = total,
                    Items = items,
                });
        }

        return receipts;
    }
}
