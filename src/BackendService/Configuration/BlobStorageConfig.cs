namespace BackendService.Configuration;

public class BlobStorageConfig
{
    public static readonly string SectionName = "BlobStorage";

    public string ConnectionString { get; init; } = default!;
    public string StorageAccountName { get; init; } = default!;
    public string ReceiptsContainerName { get; init; } = default!;
}
