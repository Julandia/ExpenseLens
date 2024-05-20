namespace Deploy.Infrastructure.Configuration;

public record Environment(string Name, bool MultiRegionSupport, bool IsPrimary);
