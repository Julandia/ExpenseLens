using System.Collections.Immutable;
using Deploy.Infrastructure.Configuration;
using Pulumi;

namespace Deploy.Infrastructure.Exports;

public class InfrastructureStackExports : IInfrastructureStackExports
{
    private readonly DeploymentConfig _config;
    private readonly ImmutableDictionary<string, object> _infraOutputs;

    public InfrastructureStackExports(DeploymentConfig config)
    {
        _config = config;
        var stackReference = new StackReference($"organization/AWS.Infrastructure/{config.DeploymentEnvironment.Name}");

        // TODO find a better way of doing that if possible
        var infraStackOutput = stackReference.GetValueAsync("stack-outputs").GetAwaiter().GetResult();
        _infraOutputs = (ImmutableDictionary<string, object>)infraStackOutput!;
    }

    public string? GetStringValue(string resourceName, string resourceOutputKey)
        => GetValue<string>(resourceName, resourceOutputKey);

    public string? GetGlobalStringValue(string resourceName, string resourceOutputKey)
        => GetGlobalValue<string>(resourceName, resourceOutputKey);

    public string? GetPerRegionStringValue(string resourceName, string resourceOutputKey)
        => GetPerRegionValue<string>(resourceName, resourceOutputKey);

    public string GetRequiredStringValue(string resourceName, string resourceOutputKey)
        => GetValue<string>(resourceName, resourceOutputKey) ?? throw new Exception($"Failed to find stack output for resource {resourceName} and key: {resourceOutputKey}");

    public string GetGlobalRequiredStringValue(string resourceName, string resourceOutputKey)
        => GetGlobalValue<string>(resourceName, resourceOutputKey) ?? throw new Exception($"Failed to find stack output for resource {resourceName} and key: {resourceOutputKey}");

    public string GetPerRegionRequiredStringValue(string resourceName, string resourceOutputKey)
        => GetPerRegionValue<string>(resourceName, resourceOutputKey) ?? throw new Exception($"Failed to find stack output for resource {resourceName} and key: {resourceOutputKey}");

    public string[]? GetStringArrayValue(string resourceName, string resourceOutputKey)
    {
        var valueAsObject = GetValue<object>(resourceName, resourceOutputKey);

        var valueAsImmutableArray = (ImmutableArray<object>?) valueAsObject;
        return valueAsImmutableArray?.Select(x => (string)x).ToArray();
    }

    public string[]? GetGlobalStringArrayValue(string resourceName, string resourceOutputKey)
    {
        var valueAsObject = GetGlobalValue<object>(resourceName, resourceOutputKey);

        var valueAsImmutableArray = (ImmutableArray<object>?) valueAsObject;
        return valueAsImmutableArray?.Select(x => (string)x).ToArray();
    }

    public string[]? GetPerRegionStringArrayValue(string resourceName, string resourceOutputKey)
    {
        var valueAsObject = GetPerRegionValue<object>(resourceName, resourceOutputKey);

        var valueAsImmutableArray = (ImmutableArray<object>?) valueAsObject;
        return valueAsImmutableArray?.Select(x => (string)x).ToArray();
    }

    public T? GetValue<T>(string resourceName, string resourceOutputKey) where T : class
        => InternalGetValue<T>($"{resourceName}-{_config.DeploymentEnvironment.CurrentEnvironment.Name}-{_config.DeploymentEnvironment.CurrentRegion.Region}", resourceOutputKey);

    public T? GetGlobalValue<T>(string resourceName, string resourceOutputKey) where T : class
        => InternalGetValue<T>(resourceName, resourceOutputKey);

    public T? GetPerRegionValue<T>(string resourceName, string resourceOutputKey) where T : class
        => InternalGetValue<T>($"{resourceName}-{_config.DeploymentEnvironment.Name}-{_config.DeploymentEnvironment.CurrentRegion.Region}", resourceOutputKey);

    private T? InternalGetValue<T>(string resourceName, string resourceOutputKey) where T : class
    {
        if (!_infraOutputs.TryGetValue(resourceName, out var resourcesOutputsObject))
        {
            return null;
        }

        if (resourcesOutputsObject is not ImmutableDictionary<string, object> resourcesOutputs)
        {
            return null;
        }

        if (!resourcesOutputs.TryGetValue(resourceOutputKey, out var value))
        {
            return null;
        }

        return value as T;
    }
}
