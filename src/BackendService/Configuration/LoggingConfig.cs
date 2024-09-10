namespace BackendService.Configuration;

public class LoggingConfig
{
    public static readonly string SectionName = "Logging";

    public string ApplicationInsightsConnectionString { get; init; } = default!;
}
