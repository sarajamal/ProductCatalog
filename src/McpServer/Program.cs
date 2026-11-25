using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductCatalog.McpServer;

var builder = Host.CreateApplicationBuilder(args);

// stdout is reserved for MCP messages, so every log line goes to stderr.
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Base URL of ProductCatalogApi. Override with the environment variable
// ProductCatalogApi__BaseUrl (for example in claude_desktop_config.json).
var baseUrl = builder.Configuration["ProductCatalogApi:BaseUrl"] ?? "http://localhost:5233";

builder.Services.AddHttpClient<ProductCatalogClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
