using Azure.Monitor.OpenTelemetry.Exporter;
using BackendService.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
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
            var loggingConfig = serviceProvider.GetRequiredService<IOptions<LoggingConfig>>().Value;
            loggerConfiguration
                .WriteTo.ApplicationInsights(new TelemetryConfiguration { ConnectionString = loggingConfig.ApplicationInsightsConnectionString },TelemetryConverter.Traces)
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceVersion", serviceInfo.Version)
                .Enrich.WithProperty("ServiceName", serviceInfo.Name)
                .Enrich.WithProperty("Project", serviceInfo.Project)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Information)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning);
        });
    }

    // Don't know whether it works or not
    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddAzureMonitorTraceExporter()
            .Build();
        var metricsProvider = Sdk.CreateMeterProviderBuilder()
            .AddAzureMonitorMetricExporter()
            .Build();
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.AddAzureMonitorLogExporter();
            });
        });
        services.AddSingleton<ILoggerFactory>(loggerFactory);
    }
}
