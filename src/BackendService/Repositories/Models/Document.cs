using System.Text.Json.Serialization;

namespace BackendService.Repositories.Models;

public class Document
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Category { get; init; } = DocumentCategory.Other.ToString();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
