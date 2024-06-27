using Azure.Monitor.OpenTelemetry.Exporter;
using BackendService.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using Serilog;
using Serilog.Events;

namespace BackendService.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddExpenseLensLogging(this IServiceCollection services)
    {
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddAzureMonitorTraceExporter();
        var metricsProvider = Sdk.CreateMeterProviderBuilder()
            .AddAzureMonitorMetricExporter();

        // Create a new logger factory.
        // It is important to keep the LoggerFactory instance active throughout the process lifetime.
        // See https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/docs/logs#logger-management
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.AddAzureMonitorLogExporter();
            });
        });
        services.AddSingleton<ILoggerFactory>(loggerFactory);

        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var serviceInfo = serviceProvider.GetRequiredService<IOptions<ServiceInfoConfig>>().Value;
            loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.WithProperty("ServiceVersion", serviceInfo.Version)
                .Enrich.WithProperty("ServiceName", serviceInfo.Name)
                .Enrich.WithProperty("Project", serviceInfo.Project)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning);
        });
    }
}
