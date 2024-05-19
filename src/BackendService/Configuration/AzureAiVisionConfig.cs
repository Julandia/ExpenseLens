namespace BackendService.Configuration;

public class AzureAiVisionConfig
{
    public static readonly string SectionName = "AzureAiVision";
    public string Endpoint { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
}
