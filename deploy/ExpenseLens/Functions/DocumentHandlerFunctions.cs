using Deploy.ExpenseLens.BackendService;
using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace Deploy.ExpenseLens.Functions;

public class DocumentHandlerFunctions : IPulumiRegionalResource
{
    public DocumentHandlerFunctions(DeploymentConfig config,
        FunctionResourceNames names,
        DocumentStorageAccount documentStorageAccount,
        BackendContainerApps backendContainerApps,
        ExpenseLensResourceGroup resourceGroup,
        ApplicationInsights appInsights)
    {
        var appServicePlan = new AppServicePlan(names.AppServicePlanName, new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Sku = new SkuDescriptionArgs
            {
                Name = "Y1",
                Tier = "Dynamic",
            },
        });


        var functionApp = new WebApp(names.FunctionAppName, new WebAppArgs
        {
            Kind = "FunctionApp",
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ServerFarmId = appServicePlan.Id,
            SiteConfig = new SiteConfigArgs
            {
                AppSettings =
                {
                    new NameValuePairArgs
                    {
                        Name = "AzureWebJobsStorage",
                        Value = documentStorageAccount.ConnectionString,
                    },
                    new NameValuePairArgs
                    {
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet-isolated",
                    },
                    new NameValuePairArgs
                    {
                        Name = "FUNCTIONS_EXTENSION_VERSION",
                        Value = "~4",
                    },
                    new NameValuePairArgs
                    {
                        Name = "ExpenseLensServiceHost",
                        Value = backendContainerApps.Endpoint,
                    },
                    new NameValuePairArgs
                    {
                        Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                        Value = appInsights.InstrumentationKey,
                    }
                },
                Http20Enabled = true,
                Use32BitWorkerProcess = false,
            },
            HttpsOnly = true,
        });

        var logConfig = new WebAppDiagnosticLogsConfiguration(names.FunctionAppLogName, new WebAppDiagnosticLogsConfigurationArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Name = functionApp.Name,
            HttpLogs = new HttpLogsConfigArgs
            {
                FileSystem = new FileSystemHttpLogsConfigArgs
                {
                    Enabled = true,
                    RetentionInMb = 100,
                    RetentionInDays = 7
                },
            },
            ApplicationLogs = new ApplicationLogsConfigArgs
            {
                FileSystem = new FileSystemApplicationLogsConfigArgs
                {
                    Level = LogLevel.Information,
                },
            },
        });
    }
}
