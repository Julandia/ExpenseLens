using Azure.Storage.Blobs;
using BackendService.Configuration;
using Microsoft.Extensions.Options;

namespace BackendService.Repositories.FileStorage;

public class BlobStorage : IFileStorage
{
    private readonly BlobStorageConfig _blobStorageConfig;
    private readonly BlobContainerClient _receiptContainer;

    public BlobStorage(IOptions<BlobStorageConfig> blobStorageOptions)
    {
        _blobStorageConfig = blobStorageOptions.Value;
        _receiptContainer = new BlobContainerClient(blobStorageOptions.Value.ConnectionString, blobStorageOptions.Value.ReceiptsContainerName);
        _receiptContainer.CreateIfNotExists();
    }

    public async Task<bool> CheckIfReceiptFileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
       return await _receiptContainer.GetBlobClient(fileName).ExistsAsync(cancellationToken);
    }
}
