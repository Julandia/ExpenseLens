using Deploy.ExpenseLens.Shared;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Configuration;
using Pulumi;
using Pulumi.AzureNative.DocumentDB;
using Pulumi.AzureNative.DocumentDB.Inputs;

namespace Deploy.ExpenseLens.BackendService;

public class CosmosDatabase : IPulumiRegionalResource
{
     public Output<string> Endpoint { get; }
     public Output<string> ConnectionString { get; }

     public CosmosDatabase(DeploymentConfig config,
         BackendServiceResourceNames names,
         ExpenseLensResourceGroup resourceGroup)
     {
         var cosmosdbAccount = new DatabaseAccount(names.CosmosDatabaseAccountName, new()
         {
             ResourceGroupName = resourceGroup.Name,
             Location = resourceGroup.Location,
             DatabaseAccountOfferType = DatabaseAccountOfferType.Standard,
             Locations = new[]
             {
                 new LocationArgs
                 {
                     LocationName = resourceGroup.Location,
                     FailoverPriority = 0,
                 },
             },
             ConsistencyPolicy = new ConsistencyPolicyArgs
             {
                 DefaultConsistencyLevel = DefaultConsistencyLevel.Session,
             },
             // Capabilities = new[]
             // {
             //     new CapabilityArgs
             //     {
             //         Name = "EnableMultiRegionWrites",
             //     },
             // },
             EnableFreeTier = true,
         });

         var db = new SqlResourceSqlDatabase(names.CosmosDatabaseName, new SqlResourceSqlDatabaseArgs
         {
             ResourceGroupName = resourceGroup.Name,
             AccountName = cosmosdbAccount.Name,
             Resource = new SqlDatabaseResourceArgs
             {
                 Id = names.CosmosDatabaseName,
             },
         });

         // Cosmos DB SQL Container
         var dbContainer = new SqlResourceSqlContainer(names.CosmosDatabaseDocumentsContainerName, new SqlResourceSqlContainerArgs
         {
             ResourceGroupName = resourceGroup.Name,
             AccountName = cosmosdbAccount.Name,
             DatabaseName = db.Name,
             Resource = new SqlContainerResourceArgs
             {
                 Id = names.CosmosDatabaseDocumentsContainerName,
                 PartitionKey = new ContainerPartitionKeyArgs { Paths = {"/category" }, Kind = "Hash"},
             },
         });

         Endpoint = cosmosdbAccount.DocumentEndpoint;
         ConnectionString = Output.All(resourceGroup.Name, cosmosdbAccount.Name).Apply(async values =>
         {
             var resourceGroupName = values[0];
             var accountName = values[1];
             var result = await ListDatabaseAccountConnectionStrings.InvokeAsync(new ListDatabaseAccountConnectionStringsArgs
             {
                 ResourceGroupName = resourceGroupName,
                 AccountName = accountName,
             });
             return result.ConnectionStrings[0].ConnectionString;
         });
    }
}
