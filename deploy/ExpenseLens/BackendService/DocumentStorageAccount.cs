using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace Deploy.ExpenseLens.BackendService;

public class DocumentStorageAccount : IPulumiRegionalResource
{
    public Output<string> Name { get; }
    public Output<string> ConnectionString { get; }
    public Output<string> ReceiptContainerName { get; }

    public DocumentStorageAccount(DeploymentConfig config,
        BackendServiceResourceNames names,
        ExpenseLensResourceGroup resourceGroup,
        DocumentIntelligenceService documentIntelligenceService)
    {
        var storageAccount = new StorageAccount(names.DocumentStorageAccountName, new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS,
            },
            Kind = Kind.StorageV2,
            Location = resourceGroup.Location,
        });

        var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountName = storageAccount.Name,
        });

        var primaryStorageKey = storageAccountKeys.Apply(keys => keys.Keys[0].Value);

        var receiptContainer = new BlobContainer(names.DocumentStorageReceiptContainerName, new BlobContainerArgs
        {
            AccountName = storageAccount.Name,
            ResourceGroupName = resourceGroup.Name,
            PublicAccess = PublicAccess.None
        });

        var roleAssignment = new RoleAssignment(names.DocumentStorageReaderRoleAssignmentName, new RoleAssignmentArgs
        {
            Scope = storageAccount.Id,
            RoleDefinitionId = $"/subscriptions/{config.DeploymentEnvironment.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/2a2b9908-6ea1-4ae2-8e65-a410df84e7d1", // Role definition ID for "Storage Blob Data Reader"
            PrincipalId = documentIntelligenceService.PrincipalId,
            PrincipalType = "ServicePrincipal",
        });

        Name = storageAccount.Name;
        ConnectionString = Output.Tuple(storageAccount.Name, primaryStorageKey).Apply(t =>
        {
            var (accountName, accountKey) = t;
            return $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";
        });
        ReceiptContainerName = receiptContainer.Name;
    }
}
