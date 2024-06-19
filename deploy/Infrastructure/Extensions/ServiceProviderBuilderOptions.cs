using Microsoft.Extensions.DependencyInjection;

namespace Deploy.Infrastructure.Extensions;

public class ServiceProviderBuilderOptions
{
    /// <summary>
    /// Controls whether or not secrets will be decrypted based on actual values in pulumi.{stack}.yaml file.
    /// Set it to false _only_ for unit tests where values in that config can't be decrypted.
    /// </summary>
    public bool UsePulumiConfigDecryption { get; set; } = true;

    /// <summary>
    /// Additional setup of the service collection.
    /// </summary>
    public Action<IServiceCollection>? ConfigureServices { get; set; }

    /// <summary>
    /// Path to the directory containing config files, relative to the root folder of the repository.
    /// E.g /.submodules/deploy/configs.
    /// </summary>
    public string? ConfigsPath { get; set; }

    /// <summary>
    /// Do not change this value outside of infrastructure repository.
    /// If you do you'll most likely have conflicts when trying to deploy your stack.
    /// </summary>
    public bool SkipPulumiSetup { get; set; } = true;

    /// <summary>
    /// If set to true (default) it will register InfrastructureStackExports type in IoC that can be used to get infrastructure repo outputs
    /// </summary>
    public bool RegisterInfrastructureStackExports { get; set; } = true;
}
