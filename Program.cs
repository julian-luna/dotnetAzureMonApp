// Adapted from OpenTelemetry Samples: 
// https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/logs/getting-started-aspnetcore. 

using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Clear default logging providers to be able to select OTel exporters
builder.Logging.ClearProviders();

builder.Logging.AddOpenTelemetry(logging =>
{
    var resourceBuilder = ResourceBuilder
        .CreateDefault()
        .AddService(builder.Environment.ApplicationName);

    logging.SetResourceBuilder(resourceBuilder)
        
        // Default to exporting to console
        // .AddConsoleExporter();
        // Export to Azure Monitor, providing AppInsights connection string
        // Comment out the Console exporter and uncomment below, replacing the connection string of the AppInsights workspace to use Azure Monitor
        .AddAzureMonitorLogExporter(o => o.ConnectionString = "InstrumentationKey=4eeeb75d-fd81-4a0a-aa48-ba2ba4588682;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/");

});

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.FoodPriceChanged("artichoke", 9.99);

    return "Hello from OpenTelemetry Logs under Docker!";
});

app.Logger.StartingApp();

app.Run();

internal static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "Starting the app...")]
    public static partial void StartingApp(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Food `{name}` price changed to `{price}`.")]
    public static partial void FoodPriceChanged(this ILogger logger, string name, double price);
}