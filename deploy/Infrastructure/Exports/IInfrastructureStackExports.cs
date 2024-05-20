namespace Deploy.Infrastructure.Exports;

public interface IInfrastructureStackExports
{
    string? GetStringValue(string resourceName, string resourceOutputKey);
    string? GetGlobalStringValue(string resourceName, string resourceOutputKey);
    string? GetPerRegionStringValue(string resourceName, string resourceOutputKey);

    string GetRequiredStringValue(string resourceName, string resourceOutputKey);
    string GetGlobalRequiredStringValue(string resourceName, string resourceOutputKey);
    string GetPerRegionRequiredStringValue(string resourceName, string resourceOutputKey);

    string[]? GetStringArrayValue(string resourceName, string resourceOutputKey);
    string[]? GetGlobalStringArrayValue(string resourceName, string resourceOutputKey);
    string[]? GetPerRegionStringArrayValue(string resourceName, string resourceOutputKey);

    T? GetValue<T>(string resourceName, string resourceOutputKey) where T : class;
    T? GetGlobalValue<T>(string resourceName, string resourceOutputKey) where T : class;
    T? GetPerRegionValue<T>(string resourceName, string resourceOutputKey) where T : class;
}
