using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;

namespace Deploy.ExpenseLens.Functions;

public class FunctionResourceNames : ResourceNames
{
    public string AppServicePlanName => Resolve("expense-lens-function-plan");
    public string FunctionAppName => Resolve("expense-lens-function-app");
    public string FunctionAppLogName => Resolve("expense-lens-function-app-log");
    public string AppInsightsName => Resolve("expense-lens-function-app-insights");

    public FunctionResourceNames(DeploymentConfig config) : base(config)
    {
    }
}
