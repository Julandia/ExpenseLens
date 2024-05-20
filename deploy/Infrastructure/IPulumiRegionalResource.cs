namespace Deploy.Infrastructure;

/// <summary>
/// When class implements this interface and declares any cloud resources in it these resources will be created for each region for each environment (e.g development, dev-myprivateenv).
///
/// </summary>
public interface IPulumiRegionalResource
{
    PulumiResourceOutput GetOutputs()
    {
        return PulumiResourceOutput.Empty;
    }
}

/// <summary>
/// When class implements this interface and declares any cloud resources in it these resources will be created only once per deployment environment (e.g development, qa and production).
/// </summary>
public interface IPulumiGlobalResource
{
    PulumiResourceOutput GetOutputs()
    {
        return PulumiResourceOutput.Empty;
    }
}

public record PulumiResourceOutput(string ResourceName, IDictionary<string, object> Outputs, bool IsPerRegionOutput = false)
{
    public static readonly PulumiResourceOutput Empty = new(string.Empty, new Dictionary<string, object>());
}
