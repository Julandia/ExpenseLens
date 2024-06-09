using Deploy.Infrastructure;
using Deploy.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Deployment = Pulumi.Deployment;

var serviceProvider = ServiceProviderExtensions.CreateServiceProvider<Program>(new ServiceProviderBuilderOptions
{
    UsePulumiConfigDecryption = true,
    ConfigureServices = services =>
    {
        services.AddSingleton<PulumiStack>();
    },
    ConfigsPath = "/configs",
    SkipPulumiSetup = false,
    RegisterInfrastructureStackExports = false,
});

await Deployment.RunAsync<PulumiStack>(serviceProvider);
