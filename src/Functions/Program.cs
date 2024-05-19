using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Functions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient("ExpenseLens", (sp, client) =>
        {
            var expenseLensHost = Environment.GetEnvironmentVariable("ExpenseLensServiceHost") ??
                        throw new Exception("Missing ExpenseLensServiceHost in environment variable");
            client.BaseAddress = new Uri(expenseLensHost);
        });
        services.AddScoped<IExpenseLensServiceClient>(sp =>
        {
            var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExpenseLens");
            return RestService.For<IExpenseLensServiceClient>(client);
        });
    })
    .Build();

host.Run();
