using Deploy.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deploy.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceProvider CreateServiceProvider<TAssemblyType>(ServiceProviderBuilderOptions options)
    {
        var services = new ServiceCollection();
        services.AddSingleton(_ => ConfigReader.LoadConfig(options.ConfigsPath));

        AddPulumiResourcesClasses<TAssemblyType, IPulumiRegionalResource>(services, ServiceLifetime.Scoped);
        AddPulumiResourcesClasses<TAssemblyType, IPulumiGlobalResource>(services, ServiceLifetime.Singleton);

        services.Scan(selector =>
        {
            selector.FromAssemblyOf<ResourceNames>()
                .AddClasses(classes => classes.AssignableTo<ResourceNames>())
                .AsSelf()
                .WithSingletonLifetime();
        });

        services.Scan(selector =>
        {
            selector.FromAssemblyOf<TAssemblyType>()
                .AddClasses(classes => classes.AssignableTo<ResourceNames>())
                .AsSelf()
                .WithSingletonLifetime();
        });

        options.ConfigureServices?.Invoke(services);

        return services.BuildServiceProvider();
    }

    private static void AddPulumiResourcesClasses<TAssemblyType, TInterface>(IServiceCollection services, ServiceLifetime lifetime)
    {
        var pulumiResources = typeof(TAssemblyType).Assembly.DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(TInterface))).ToList();
        foreach (var resourceType in pulumiResources)
        {
            if (typeof(IPulumiRegionalResource).IsAssignableFrom(resourceType) && typeof(IPulumiGlobalResource).IsAssignableFrom(resourceType))
            {
                throw new Exception($"Type '{resourceType.Name}' implements both '{nameof(IPulumiRegionalResource)}' and '{nameof(IPulumiGlobalResource)}', this is not allowed.");
            }

            // register each class at itself but also register it as TInterface so that it can be resolved by the DI container
            services.Add(new ServiceDescriptor(resourceType.AsType(), resourceType.AsType(), lifetime));
            services.Add(new ServiceDescriptor(typeof(TInterface), provider => provider.GetRequiredService(resourceType.AsType()), lifetime));
        }
    }

    // private static IServiceCollection RegisterPulumiSharedResources(this IServiceCollection services,
    //     bool usePulumiConfigDecryption, bool skipPulumiSetup)
    // {
    //     if (!skipPulumiSetup)
    //     {
    //         services.AddSingleton<PulumiDeployGitHubRole>()
    //             .AddSingleton<IPulumiGlobalResource, PulumiDeployGitHubRole>()
    //             .AddSingleton<PulumiEncryptionKey>()
    //             .AddSingleton<IPulumiGlobalResource, PulumiEncryptionKey>()
    //     }
    //
    //     return services.AddSingleton<PulumiCustomProviders>()
    //         .AddSingleton<PulumiConfigDecorator>(_ => new PulumiConfigDecorator(usePulumiConfigDecryption));
    // }
}
