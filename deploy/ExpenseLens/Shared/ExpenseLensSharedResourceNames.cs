using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;

namespace Deploy.ExpenseLens.Shared;

public class ExpenseLensSharedResourceNames : ResourceNames
{
    public string ContainerRegistryName => Resolve("expenselens");
    public string ContainerRegistryAccessUserIdentityName => Resolve("expenselens-acr-access-identity");
    public string ContainerRegistryAccessRoleAssignmentName => Resolve("expenselens-acr-access-role-assignment");
    public string ExpenseLensSharedResourceGroupName => Resolve("expense-lens-rg-{env}");

    public ExpenseLensSharedResourceNames(DeploymentConfig config) : base(config)
    {
    }
}
