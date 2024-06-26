using Refit;

namespace ExpenseLens.Functions;

public interface IExpenseLensServiceClient
{
    [Headers("Content-Type:application/json")]
    [Post("/api/v1/Scanner/receipts")]
    Task<HttpResponseMessage> ScanReceipt([Body]object fileName);

}
