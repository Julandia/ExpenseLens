using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;

namespace Deploy.ExpenseLens.BackendService;

public class BackendServiceResourceNames : ResourceNames
{
    public string DocumentStorageAccountName => Resolve("lensdocuments");
    public string DocumentStorageReceiptContainerName => Resolve("receipts");

    public string DocumentStorageReaderRoleAssignmentName => Resolve("lensdocuments-reader-role-assignment");
    public string BackendContainerAppEnvironmentName => Resolve("expense-lens-backend-env");
    public string BackendContainerAppName => Resolve("expense-lens-backend-app");
    public string BackendContainerName => Resolve("expense-lens-backend-container");
    public string BackendContainerRepositoryName => Resolve("expense-lens-service");
    public string BackendContainerImageVersion => System.Environment.GetEnvironmentVariable("BACKEND_SERVICE_IMAGE_VERSION") ?? "37663a75b00a2ecf71fba97c5e5fcc80fb9fd817";
    public string DocumentIntelligenceServiceName => Resolve("expense-lens-doc-intelligence-service");

    public string CosmosDatabaseAccountName => Resolve("expense-lens-db-account");
    public string CosmosDatabaseName => Resolve("expense-lens-db");
    public string CosmosDatabaseDocumentsContainerName => Resolve("documents");

    public BackendServiceResourceNames(DeploymentConfig config) : base(config)
    {
    }
}
