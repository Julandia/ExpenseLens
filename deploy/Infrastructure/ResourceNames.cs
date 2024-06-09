using System.Text.RegularExpressions;
using Deploy.Infrastructure.Configuration;

namespace Deploy.Infrastructure;

public abstract class ResourceNames
{
    private readonly Regex _missingParametersRegex = new("{(.*?)}");

    protected DeploymentConfig Config { get; }

    protected ResourceNames(DeploymentConfig config)
    {
        Config = config;
    }

    protected string Resolve(string template)
    {
        var updatedTemplate = template
            .Replace("{env}", Config.DeploymentEnvironment.CurrentEnvironment.Name)
            .Replace("{deploymentEnv}", Config.DeploymentEnvironment.Name)
            .Replace("{region}", Config.DeploymentEnvironment.CurrentRegion.Name)
            .Replace("{resourceGroup}", Config.DeploymentEnvironment.ResourceGroup);

        var matches = _missingParametersRegex.Matches(updatedTemplate);
        foreach (var match in matches)
        {
            var missingParameter = match.ToString();
            if (string.IsNullOrWhiteSpace(missingParameter))
            {
                continue;
            }

            var parameterValue = ResolveParameterValue(missingParameter);
            if (string.IsNullOrWhiteSpace(parameterValue))
            {
                throw new Exception($"Resource name contains unresolved parameter '{missingParameter}' and there is no value provided for it," +
                                    $" make sure to provide value for that parameter in {nameof(ResolveParameterValue)} method");
            }

            updatedTemplate = updatedTemplate.Replace(missingParameter, parameterValue);
        }

        return updatedTemplate;
    }

    protected virtual string? ResolveParameterValue(string? parameterName)
    {
        return null;
    }
}
