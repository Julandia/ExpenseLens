namespace BackendService.Configuration;

public class CosmosDbConfig
{
    public static readonly string SectionName = "CosmosDb";

    public string ConnectionString { get; init; } = default!;

    public string DatabaseName { get; init; } = default!;

    public string DocumentsContainerName { get; init; } = default!;
}
