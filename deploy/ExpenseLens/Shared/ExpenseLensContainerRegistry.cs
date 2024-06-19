using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace Deploy.ExpenseLens.Shared;

public class ExpenseLensContainerRegistry : IPulumiRegionalResource
{
    private const string AcrPullRoleId = "7f951dda-4ed3-4680-a7ca-43fe172d538d";
    public Output<string> LoginServer { get; }
    public Output<string> AccessIdentityPrincipalId { get; }

    public ExpenseLensContainerRegistry(DeploymentConfig config,
        ExpenseLensSharedResourceNames names,
        ExpenseLensResourceGroup resourceGroup)
    {
        var existingRegistry = GetRegistry.Invoke(new GetRegistryInvokeArgs
        {
            ResourceGroupName = "expenselens-admin",
            RegistryName = names.ContainerRegistryName,
        });
        LoginServer = existingRegistry.Apply(registry => registry.LoginServer);

        var userIdentity = new UserAssignedIdentity(names.ContainerRegistryAccessUserIdentityName, new UserAssignedIdentityArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
        });

        var acrPullRoleAssignment = new RoleAssignment(names.ContainerRegistryAccessRoleAssignmentName, new RoleAssignmentArgs
        {
            PrincipalId = userIdentity.PrincipalId,
            RoleDefinitionId = $"/subscriptions/{config.DeploymentEnvironment.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{AcrPullRoleId}",
            Scope = existingRegistry.Apply(cr => cr.Id),
            PrincipalType = "ServicePrincipal",
        });

        AccessIdentityPrincipalId =
            Output.Format(
                $"/subscriptions/{config.DeploymentEnvironment.SubscriptionId}/resourceGroups/{resourceGroup.Name}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{userIdentity.Name}");
    }
}
