using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Functions;
public interface IExpenseLensServiceClient
{
    [Headers("Content-Type:application/json")]
    [Post("/api/v1/Scanner/receipts")]
    Task<HttpResponseMessage> ScanReceipt([Body]object fileName);
    
}