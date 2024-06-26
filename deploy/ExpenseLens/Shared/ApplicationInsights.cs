using Deploy.Infrastructure;
using Pulumi;
using Pulumi.AzureNative.Insights;

namespace Deploy.ExpenseLens.Shared;

public class ApplicationInsights : IPulumiRegionalResource
{
    public Output<string> ConnectionString { get; }
    public Output<string> InstrumentationKey { get; }

    public ApplicationInsights(ExpenseLensSharedResourceNames names,
        ExpenseLensResourceGroup resourceGroup)
    {
        var appInsights = new Component(names.ExpenseLensAppInsightsResourceNames, new ComponentArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ApplicationType = ApplicationType.Web,
            IngestionMode = IngestionMode.ApplicationInsights,
            Kind = "web",
        });

        ConnectionString = appInsights.ConnectionString;
        InstrumentationKey = appInsights.InstrumentationKey;
    }
}
