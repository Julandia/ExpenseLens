namespace BackendService.Repositories.FileStorage;

public interface IFileStorage
{
    Task<bool> CheckIfReceiptFileExistsAsync(string fileName, CancellationToken cancellationToken = default);
}
