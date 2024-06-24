using BackendService.Configuration;
using BackendService.Repositories.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using CosmosDatabase = Microsoft.Azure.Cosmos.Database;

namespace BackendService.Repositories.Database;

public class CosmosDbRepository : IExpenseRepository
{
    private readonly CosmosClient _dbClient;
    private CosmosDatabase? _database;
    private Container _documentsContainer;

    public CosmosDbRepository(CosmosClient client, IOptions<CosmosDbConfig> options)
    {
        _dbClient = client;
        _database = _dbClient.GetDatabase(options.Value.DatabaseName);
        _documentsContainer = _database.GetContainer(options.Value.DocumentsContainerName);
    }

    public Task SaveReceiptsAsync(IEnumerable<Receipt> receipts, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        var response = await _documentsContainer.UpsertItemAsync(
            item: receipt,
            partitionKey: new PartitionKey(DocumentCategory.Receipt.ToString()),
            cancellationToken: cancellationToken
        );
    }

    public async Task<Receipt?> GetReceiptAsync(string receiptId, CancellationToken cancellationToken = default)
    {
        var response = await _documentsContainer.ReadItemAsync<Receipt>(
            partitionKey: new PartitionKey(DocumentCategory.Receipt.ToString()),
            id: receiptId.ToString()
        );
        return response.Resource;
    }

    public Task<IEnumerable<Receipt>> GetReceiptsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
