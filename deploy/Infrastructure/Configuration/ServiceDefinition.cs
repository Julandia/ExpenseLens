namespace Deploy.Infrastructure.Configuration;

public record ServiceDefinition
{
    public required string Name { get; set; }
    public required string RepoName { get; set; }
    public List<string> EcrRepositories { get; set; } = new();
}
