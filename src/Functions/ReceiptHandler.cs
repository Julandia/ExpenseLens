using Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ExpenseLens.Functions
{
    public class ReceiptHandler
    {
        private readonly ILogger<ReceiptHandler> _logger;
        private readonly IExpenseLensServiceClient _expenseLensServiceClient;

        public ReceiptHandler(ILogger<ReceiptHandler> logger, IExpenseLensServiceClient expenseLensServiceClient)
        {
            _logger = logger;
            _expenseLensServiceClient = expenseLensServiceClient;
        }

        [Function(nameof(ReceiptHandler))]
        public async Task Run([BlobTrigger("receipt/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            _logger.LogInformation("Blob trigger function Processing blob\n {Name}", name);
            var response = await _expenseLensServiceClient.ScanReceipt(new { fileName = name});
            if (!response.IsSuccessStatusCode) {
                string errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Blob trigger function failed to scan receipt\n Name: {Name} \n Response status code: {StatusCode} \n {ErrorDetail}", name, response.StatusCode, errorMessage);
                return;
            }
            _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {Name} \n Response status code: {StatusCode}", name, response.StatusCode);
        }
    }
}
