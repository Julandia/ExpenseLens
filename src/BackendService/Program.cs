using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using BackendService.Configuration;
using BackendService.Extensions;
using BackendService.Features.Infrastructure;
using BackendService.Repositories.Database;
using BackendService.Repositories.FileStorage;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddOptions<AzureAiVisionConfig>().Bind(builder.Configuration.GetSection(AzureAiVisionConfig.SectionName));
builder.Services.AddOptions<BlobStorageConfig>().Bind(builder.Configuration.GetSection(BlobStorageConfig.SectionName));
builder.Services.AddOptions<CosmosDbConfig>().Bind(builder.Configuration.GetSection(CosmosDbConfig.SectionName));
builder.Services.AddOptions<LoggingConfig>().Bind(builder.Configuration.GetSection(LoggingConfig.SectionName));
builder.Services.AddOptions<ServiceInfoConfig>().Bind(builder.Configuration.GetSection(ServiceInfoConfig.SectionName));
builder.Services.AddSingleton<IExpenseRepository, CosmosDbRepository>();
builder.Services.AddSingleton<IFileStorage, BlobStorage>();

//builder.Services.AddOpenTelemetry();
builder.Services.AddExpenseLensLogging();

// Use a Singleton instance of the CosmosClient
builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var handler = new SocketsHttpHandler
    {
        // Customize this value based on desired DNS refresh timer
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    };
    var cosmosClientOptions = new CosmosClientOptions()
    {
        HttpClientFactory = () => new HttpClient(handler, disposeHandler: false),
        SerializerOptions = new CosmosSerializationOptions
        {
            IgnoreNullValues = true,
            Indented = true,
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
        }
    };

    var options = serviceProvider.GetRequiredService<IOptions<CosmosDbConfig>>();
    return new CosmosClient(options.Value.ConnectionString, cosmosClientOptions);
});


builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddConfiguredRequestHandlers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
