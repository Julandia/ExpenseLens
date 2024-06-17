using System.IO;
using Deploy.ExpenseLens.BackendService;
using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace Deploy.ExpenseLens.Functions;

public class DocumentHandlerFunctions : IPulumiRegionalResource
{
    private const string StorageReaderRoleId = "acdd72a7-3385-48ef-bd42-f606fba81ae7";

    public DocumentHandlerFunctions(DeploymentConfig config,
        FunctionResourceNames names,
        DocumentStorageAccount documentStorageAccount,
        BackendContainerApps backendContainerApps,
        ExpenseLensResourceGroup resourceGroup
    )
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

        var appInsights = new Component(names.AppInsightsName, new ComponentArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ApplicationType = "web",
            IngestionMode = "ApplicationInsights",
            Kind = "web",
        });

        // var storageAccount = new StorageAccount(names.FunctionAppStorageAccountName, new StorageAccountArgs
        // {
        //     ResourceGroupName = resourceGroup.Name,
        //     Sku = new SkuArgs
        //     {
        //         Name = SkuName.Standard_LRS,
        //     },
        //     Kind = Pulumi.AzureNative.Storage.Kind.StorageV2,
        //     Location = resourceGroup.Location,
        // });
        //
        // // Create a Blob Container
        // var codeContainer = new BlobContainer("code-container", new BlobContainerArgs
        // {
        //     AccountName = storageAccount.Name,
        //     ResourceGroupName = resourceGroup.Name,
        //     PublicAccess = PublicAccess.None
        // });
        //
        // var functionAppPublishFolder = Path.Combine(@"..\..\Publish");
        //
        // // Upload ZIP archive to the Blob Container
        // var zipBlob = new Blob(names.FunctionAppZipContainerName, new BlobArgs
        // {
        //     AccountName = storageAccount.Name,
        //     ResourceGroupName = resourceGroup.Name,
        //     ContainerName = codeContainer.Name,
        //     Source = new FileArchive(functionAppPublishFolder),
        //     Type = BlobType.Block,
        // });

        // var userIdentity = new UserAssignedIdentity(names.FunctionAppStorageAccessUserIdentityName, new UserAssignedIdentityArgs
        // {
        //     ResourceGroupName = resourceGroup.Name,
        //     Location = resourceGroup.Location,
        // });

        // var zipRoleAssignment = new RoleAssignment(names.FunctionAppStorageReaderRoleAssignmentName, new RoleAssignmentArgs
        // {
        //     PrincipalId = userIdentity.PrincipalId,
        //     RoleDefinitionId = $"/subscriptions/{config.DeploymentEnvironment.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{StorageReaderRoleId}",
        //     Scope = storageAccount.Id,
        //     PrincipalType = "ServicePrincipal",
        // });

        var functionApp = new WebApp(names.FunctionAppName, new WebAppArgs
        {
            Kind = "FunctionApp",
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ServerFarmId = appServicePlan.Id,
            // Identity = new ManagedServiceIdentityArgs
            // {
            //     UserAssignedIdentities = { userIdentity.Id },
            //     Type = Pulumi.AzureNative.Web.ManagedServiceIdentityType.UserAssigned,
            // },
            SiteConfig = new SiteConfigArgs
            {
                AppSettings =
                {
                    new NameValuePairArgs
                    {
                        Name = "AzureWebJobsStorage",
                        Value = documentStorageAccount.ConnectionString,
                    },
                    // new NameValuePairArgs
                    // {
                    //     Name = "WEBSITE_RUN_FROM_PACKAGE",
                    //     //Value = "https://expenselenspulumi.blob.core.windows.net/function/ExpenseLensFunctions-0.0.2.zip",
                    //     Value = zipBlob.Url,
                    // },
                    new NameValuePairArgs
                    {
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet-isolated",
                    },
                    new NameValuePairArgs
                    {
                        Name = "ExpenseLensServiceHost",
                        Value = backendContainerApps.Endpoint,
                    },
                    new NameValuePairArgs
                    {
                        Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                        Value = appInsights.InstrumentationKey.Apply(key => key)
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
