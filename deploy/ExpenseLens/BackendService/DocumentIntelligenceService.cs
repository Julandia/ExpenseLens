using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.CognitiveServices;
using Pulumi.AzureNative.CognitiveServices.Inputs;

namespace Deploy.ExpenseLens.BackendService;

public class DocumentIntelligenceService : IPulumiRegionalResource
{
    public Output<string> Endpoint { get; }
    public Output<string> Key { get; }
    public Output<string> PrincipalId { get; }

    public DocumentIntelligenceService(DeploymentConfig config,
        BackendServiceResourceNames names,
        ExpenseLensSharedResourceNames sharedNames,
        ExpenseLensResourceGroup resourceGroup)
    {
        var cognitiveAccount = new Account(names.DocumentIntelligenceServiceName, new AccountArgs
        {
            Kind = "FormRecognizer",
            Sku = new SkuArgs
            {
                Name = "F0"  // Free tier, for testing purposes
            },
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Properties = new AccountPropertiesArgs
            {
                // Additional properties can be set here
            },
            Identity = new IdentityArgs
            {
                Type = ResourceIdentityType.SystemAssigned,
            },
        });

        var keys = ListAccountKeys.Invoke(new ListAccountKeysInvokeArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountName = cognitiveAccount.Name,
        });

        // Does endpoint format depend on the region?
        //Endpoint = Output.Format($"https://{cognitiveAccount.Name}.cognitiveservices.azure.com/");
        Endpoint = Output.Format($"https://{config.DeploymentEnvironment.CurrentRegion.Name}.api.cognitive.microsoft.com/");
        Key = keys.Apply(k => k.Key1)!;
        PrincipalId = cognitiveAccount.Identity.Apply(identity => identity?.PrincipalId);
    }
}
