using BackendService.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace BackendService.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddExpenseLensLogging(this IServiceCollection services)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var serviceInfo = serviceProvider.GetRequiredService<IOptions<ServiceInfoConfig>>().Value;
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceVersion", serviceInfo.Version)
                .Enrich.WithProperty("ServiceName", serviceInfo.Name)
                .Enrich.WithProperty("Project", serviceInfo.Project)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning);
        });
    }
}
