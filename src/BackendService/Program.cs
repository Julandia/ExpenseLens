using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using BackendService.Configuration;
using BackendService.Features.Infrastructure;
using BackendService.Repositories;
using BackendService.Repositories.FileStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddOptions<AzureAiVisionConfig>().Bind(builder.Configuration.GetSection(AzureAiVisionConfig.SectionName));
builder.Services.AddOptions<BlobStorageConfig>().Bind(builder.Configuration.GetSection(BlobStorageConfig.SectionName));
builder.Services.AddSingleton<IExpenseRepository, InMemoryExpenseRepository>();
builder.Services.AddSingleton<IFileStorage, BlobStorage>();


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
// TODO: add swagger documentation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
