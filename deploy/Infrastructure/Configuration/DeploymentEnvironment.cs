namespace Deploy.Infrastructure.Configuration;

public class DeploymentEnvironment
{
    public required string Name { get; set; }

    public required string SubscriptionId { get; set; }

    public required string ResourceGroup { get; set; }

    public IList<DeploymentRegion> Regions { get; set; } = new List<DeploymentRegion>();

    public DeploymentRegion CurrentRegion { get; set; } = default!;

    public List<Environment> Environments { get; set; } = new();

    public Environment CurrentEnvironment { get; set; } = default!;

    public bool IsProduction => Name.Equals("production", StringComparison.OrdinalIgnoreCase);

    public bool IsQa => Name.Equals("qa", StringComparison.OrdinalIgnoreCase);

    public bool IsDevelopment => Name.Equals("development", StringComparison.OrdinalIgnoreCase);
}
