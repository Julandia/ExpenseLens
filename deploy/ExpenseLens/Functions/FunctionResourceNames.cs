using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;

namespace Deploy.ExpenseLens.Functions;

public class FunctionResourceNames : ResourceNames
{
    public string AppServicePlanName => Resolve("expense-lens-function-plan");
    public string FunctionAppName => Resolve("expense-lens-function-app");
    public string FunctionAppLogName => Resolve("expense-lens-function-app-log");
    public string AppInsightsName => Resolve("expense-lens-function-app-insights");
    public string FunctionAppStorageAccountName => Resolve("functionzip");
    public string FunctionAppZipContainerName => Resolve("function-zip.zip");
    public string FunctionAppStorageAccessUserIdentityName => Resolve("functionzip-access-user-identity");
    public string FunctionAppStorageReaderRoleAssignmentName => Resolve("functionzip-reader-role-assignment");


    public FunctionResourceNames(DeploymentConfig config) : base(config)
    {
    }
}
