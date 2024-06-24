using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;

namespace Deploy.ExpenseLens.BackendService;

public class BackendContainerApps : IPulumiRegionalResource
{
    public Output<string> Endpoint { get; }

    public BackendContainerApps(DeploymentConfig config,
        BackendServiceResourceNames names,
        ExpenseLensResourceGroup resourceGroup,
        DocumentStorageAccount documentStorageAccount,
        DocumentIntelligenceService documentIntelligenceService,
        ExpenseLensContainerRegistry containerRegistry,
        CosmosDatabase cosmosDatabase)
    {
        var managedEnvironment = new ManagedEnvironment(names.BackendContainerAppEnvironmentName, new ManagedEnvironmentArgs
        {
            EnvironmentName = names.BackendContainerAppEnvironmentName,
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ZoneRedundant = false,
            Sku = new EnvironmentSkuPropertiesArgs
            {
                Name = "Consumption",
            },
        });

        var containerApp = new ContainerApp(names.BackendContainerAppName, new ContainerAppArgs
        {
            ContainerAppName = names.BackendContainerAppName,
            ResourceGroupName = resourceGroup.Name,
            ManagedEnvironmentId = managedEnvironment.Id,
            Location = resourceGroup.Location,
            Configuration = new ConfigurationArgs
            {
                Ingress = new IngressArgs
                {
                    External = true,
                    TargetPort = 8080,
                    Transport = "auto",
                },
                Registries =
                {
                    new RegistryCredentialsArgs
                    {
                        Server = containerRegistry.LoginServer,
                        Identity = containerRegistry.AccessIdentityPrincipalId,
                    },
                },
            },
            Template = new TemplateArgs
            {
                Containers =
                {
                    new ContainerArgs
                    {
                        Name = names.BackendContainerName,
                        Image = $"expenselens.azurecr.io/{names.BackendContainerRepositoryName}:{names.BackendContainerImageVersion}",
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = 0.5,
                            Memory = "1.0Gi",
                        },
                        Env = new InputList<EnvironmentVarArgs>
                        {
                            new EnvironmentVarArgs
                            {
                                Name = "AzureAiVision__Endpoint",
                                Value =  documentIntelligenceService.Endpoint,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "AzureAiVision__ApiKey",
                                Value = documentIntelligenceService.Key,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "BlobStorage__ReceiptsContainerName",
                                Value = documentStorageAccount.ReceiptContainerName,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "BlobStorage__ConnectionString",
                                Value = documentStorageAccount.ConnectionString,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "BlobStorage__StorageAccountName",
                                Value = documentStorageAccount.Name,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "CosmosDb__ConnectionString",
                                Value = cosmosDatabase.ConnectionString,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "CosmosDb__Databasename",
                                Value = names.CosmosDatabaseName,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "CosmosDb__DocumentsContainerName",
                                Value = names.CosmosDatabaseDocumentsContainerName,
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "ASPNETCORE_ENVIRONMENT",
                                Value = config.DeploymentEnvironment.CurrentEnvironment.Name,
                            },
                        },
                    },
                },
            },
            Identity = new ManagedServiceIdentityArgs
            {
                Type = ManagedServiceIdentityType.SystemAssigned_UserAssigned,
                UserAssignedIdentities = containerRegistry.AccessIdentityPrincipalId,
            },
        });
        Endpoint = Output.Format($"https://{containerApp.LatestRevisionFqdn}");
    }
}
