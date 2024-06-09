using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace Deploy.ExpenseLens.Shared;

public class ExpenseLensResourceGroup : IPulumiRegionalResource
{
    public Output<string> Id { get; }
    public Output<string> Name { get; }
    public Output<string> Location { get; }

    public ExpenseLensResourceGroup(DeploymentConfig config, ExpenseLensSharedResourceNames names)
    {
        var resourceGroup = new ResourceGroup(names.ExpenseLensSharedResourceGroupName, new ResourceGroupArgs
        {
            Location = config.DeploymentEnvironment.CurrentRegion.Name,
            ResourceGroupName = names.ExpenseLensSharedResourceGroupName,
            Tags = config.DefaultResourceTags,
        });

        Id = resourceGroup.Id;
        Name = resourceGroup.Name;
        Location = resourceGroup.Location;
    }
}
