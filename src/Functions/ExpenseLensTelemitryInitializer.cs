using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ExpenseLens.Functions;

public class ExpenseLensTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties["service.name"] = "Functions";
        telemetry.Context.GlobalProperties["service.namespace"] = "ExpenseLens";
    }
}
