namespace Deploy.Infrastructure.Configuration;

public record DeploymentConfig(string Stack)
{
    public required List<DeploymentEnvironment> AllDeploymentEnvironments { get; init; }

    public DeploymentEnvironment DeploymentEnvironment { get; init; } = default!;

    public required Dictionary<string, string> DefaultResourceTags { get; init; } = new();
    public required List<ServiceDefinition> Services { get; init; }
}
