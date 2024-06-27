namespace BackendService.Configuration;

public class ServiceInfoConfig
{
    public static readonly string SectionName = "ServiceInfo";

    public string Version { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Project { get; init; } = "ExpenseLens";
}
