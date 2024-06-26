using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace BackendService;

public class ExpenseLensTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties["service.name"] = "Backend";
        telemetry.Context.GlobalProperties["service.namespace"] = "ExpenseLens";
    }
}
