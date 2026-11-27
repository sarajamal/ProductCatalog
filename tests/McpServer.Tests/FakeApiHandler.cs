using System.Net;
using System.Text;
using System.Text.Json;
using ProductCatalog.McpServer.Models;

namespace ProductCatalog.McpServer.Tests;

/// <summary>
/// A fake ProductCatalogApi. Each test sets the answer for "METHOD path"
/// (for example "GET api/products") and can read back every request it received.
/// No network, no database.
/// </summary>
public sealed class FakeApiHandler : HttpMessageHandler
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    private readonly Dictionary<string, Func<HttpResponseMessage>> _routes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Requests received, in order: method, path and body text.</summary>
    public List<(string Method, string Path, string? Body)> Requests { get; } = new();

    /// <summary>When set, every request throws this (simulates the API being down or slow).</summary>
    public Exception? ThrowOnSend { get; set; }

    public FakeApiHandler Returns(string method, string path, HttpStatusCode status, object? body = null)
    {
        _routes[$"{method} {path}"] = () =>
        {
            var response = new HttpResponseMessage(status);
            if (body is not null)
            {
                var text = body as string ?? JsonSerializer.Serialize(body, Json);
                var type = body is string && !text.TrimStart().StartsWith('{') ? "text/plain" : "application/json";
                response.Content = new StringContent(text, Encoding.UTF8, type);
            }
            return response;
        };
        return this;
    }

    public FakeApiHandler Ok(string method, string path, object body) => Returns(method, path, HttpStatusCode.OK, body);

    /// <summary>Requests that match "METHOD path".</summary>
    public IEnumerable<(string Method, string Path, string? Body)> Sent(string method, string path) =>
        Requests.Where(r => r.Method.Equals(method, StringComparison.OrdinalIgnoreCase) &&
                            r.Path.Equals(path, StringComparison.OrdinalIgnoreCase));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri!.AbsolutePath.TrimStart('/');
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add((request.Method.Method, path, body));

        if (ThrowOnSend is not null) throw ThrowOnSend;

        return _routes.TryGetValue($"{request.Method.Method} {path}", out var make)
            ? make()
            : new HttpResponseMessage(HttpStatusCode.NotFound);
    }
}

/// <summary>Sample data and a ready client wired to a <see cref="FakeApiHandler"/>.</summary>
public static class TestApi
{
    public const string BaseUrl = "http://fake-api.test/";

    public static readonly Category Electronics = new() { CategoryId = 1, CategoryName = "Electronics" };
    public static readonly Category Books = new() { CategoryId = 2, CategoryName = "Books" };
    public static readonly Category Clothing = new() { CategoryId = 3, CategoryName = "Clothing" };

    public static List<Category> Categories() => new() { Electronics, Books, Clothing };

    // Same shape as the real data: two products under 20 (the red alert in the Angular app).
    public static List<Product> Products() => new()
    {
        new() { ProductId = 1, ProductName = "Wireless Mouse", Price = 25.50m, Stock = 97, CategoryId = 1, Category = Electronics },
        new() { ProductId = 2, ProductName = "Mechanical Keyboard", Price = 89.99m, Stock = 16, CategoryId = 1, Category = Electronics },
        new() { ProductId = 3, ProductName = "Clean Code Book", Price = 35.00m, Stock = 65, CategoryId = 2, Category = Books },
        new() { ProductId = 4, ProductName = "Product3", Price = 55.00m, Stock = 19, CategoryId = 1, Category = Electronics },
        new() { ProductId = 9, ProductName = "MCP Test Cable", Price = 10.00m, Stock = 25, CategoryId = 1, Category = Electronics }
    };

    /// <summary>A handler that already answers GET api/categories and GET api/products with the sample data.</summary>
    public static FakeApiHandler WithCatalog() => new FakeApiHandler()
        .Ok("GET", "api/categories", Categories())
        .Ok("GET", "api/products", Products());

    public static ProductCatalogClient Client(FakeApiHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });
}
