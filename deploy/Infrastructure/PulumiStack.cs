using System.Collections.Immutable;
using Deploy.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulumi;
using Environment = Deploy.Infrastructure.Configuration.Environment;

namespace Deploy.Infrastructure;

public class PulumiStack : Stack
{
    private readonly Dictionary<string, object> _outputs = new();

    [Output("stack-outputs")]
    public Output<ImmutableDictionary<string, object>> StackOutput { get; set; }

    public PulumiStack(IServiceProvider serviceProvider) : base(new StackOptions())
    {
        var config = serviceProvider.GetRequiredService<DeploymentConfig>();
        var primaryRegionsCount = config.DeploymentEnvironment.Regions.Count(x => x.IsPrimary);
        if (primaryRegionsCount != 1)
        {
            Log.Error($"Expected only one region, for deployment environment '{config.DeploymentEnvironment.Name}'" +
                      $" to be specified as 'Primary' but found '{primaryRegionsCount}' instead, please choose one and try again");
            throw new Exception("Invalid configuration");
        }

        var primaryEnvCount = config.DeploymentEnvironment.Environments.Count(x => x.IsPrimary);
        if (primaryEnvCount != 1)
        {
            Log.Error($"Expected only one environment, for deployment environment '{config.DeploymentEnvironment.Name}'" +
                      $" to be specified as 'Primary' but found '{primaryEnvCount}' instead, please choose one and try again");
            throw new Exception("Invalid configuration");
        }

        foreach (var env in config.DeploymentEnvironment.Environments.OrderByDescending(x => x.IsPrimary))
        {
            config.DeploymentEnvironment.CurrentEnvironment = env;
            foreach (var region in config.DeploymentEnvironment.Regions.OrderByDescending(x => x.IsPrimary))
            {
                // creating a scope so that all classes implementing IPulumiRegionalResource are resolved once per region
                using var serviceScope = serviceProvider.CreateScope();
                config.DeploymentEnvironment.CurrentRegion = region;

                if (region.IsPrimary)
                {
                    // resolve global resources for primary region - that way we ensure that if any other resource depends on global resource
                    // the global resource will be created in the first place
                    var globalResources = serviceProvider.GetServices<IPulumiGlobalResource>();
                    foreach (var globalResource in globalResources)
                    {
                        AppendOutputs(env, region, globalResource.GetOutputs(), false);
                    }
                }

                if (!env.MultiRegionSupport && !region.IsPrimary)
                {
                    continue;
                }

                // resolving all classes implementing IPulumiRegionalResource interface is enough to trigger each of theirs cctor which in effect will trigger also their Pulumi resources creation
                // ordering of dependencies is handled properly thanks to the DI container which makes sure that resources that other classes depend on are created first
                var regionalResources = serviceScope.ServiceProvider.GetServices<IPulumiRegionalResource>();
                foreach (var regionalResource in regionalResources)
                {
                    AppendOutputs(env, region, regionalResource.GetOutputs(), true);
                }
            }
        }

        StackOutput = Output.Create(_outputs.ToImmutableDictionary());
    }

    private void AppendOutputs(Environment env, DeploymentRegion region, PulumiResourceOutput resourceOutput, bool isRegionalResource)
    {
        if (resourceOutput == PulumiResourceOutput.Empty)
        {
            return;
        }

        if (resourceOutput.IsPerRegionOutput)
        {
            foreach (var output in resourceOutput.Outputs)
            {
                _outputs.Add(output.Key, output.Value);
            }
        }
        else
        {
            var outputSectionName = isRegionalResource ? $"{resourceOutput.ResourceName}-{env.Name}-{region.Name}" : resourceOutput.ResourceName;
            _outputs.Add(outputSectionName, resourceOutput.Outputs);
        }
    }
}
