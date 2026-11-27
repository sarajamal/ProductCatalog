using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProductCatalog.McpServer.Models;

namespace ProductCatalog.McpServer;

/// <summary>
/// Typed HTTP client for ProductCatalogApi. All MCP tools go through this class,
/// so it can be unit tested with a fake HttpMessageHandler (phase 5).
/// </summary>
public class ProductCatalogClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public ProductCatalogClient(HttpClient http)
    {
        _http = http;
    }

    public Uri? BaseAddress => _http.BaseAddress;

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<Category>>("api/categories", JsonOptions, ct);
        return result ?? new List<Category>();
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<Product>>("api/products", JsonOptions, ct);
        return result ?? new List<Product>();
    }

    /// <summary>Returns null when the API answers 404.</summary>
    public async Task<Product?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        using var response = await _http.GetAsync($"api/products/{productId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>(JsonOptions, ct);
    }

    /// <summary>POST api/products. Returns the created product, or the API's validation message.</summary>
    public async Task<(Product? Created, string? Error)> CreateProductAsync(Product product, CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync("api/products", product, JsonOptions, ct);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return (null, await ReadProblemAsync(response, ct));
        }
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Product>(JsonOptions, ct), null);
    }

    /// <summary>PUT api/products/{id}. Sends the whole product (without its Category object).</summary>
    public async Task<string?> UpdateProductAsync(Product product, CancellationToken ct = default)
    {
        var body = new Product
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
        using var response = await _http.PutAsJsonAsync($"api/products/{product.ProductId}", body, JsonOptions, ct);
        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            return await ReadProblemAsync(response, ct);
        }
        response.EnsureSuccessStatusCode();
        return null;
    }

    // Turns an ASP.NET Core ValidationProblem body into one readable line.
    private static async Task<string> ReadProblemAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var text = await response.Content.ReadAsStringAsync(ct);
        try
        {
            using var doc = JsonDocument.Parse(text);
            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(e => e.Value.EnumerateArray().Select(v => v.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m));
                return string.Join(" ", messages);
            }
            if (doc.RootElement.TryGetProperty("title", out var title))
            {
                return title.GetString() ?? $"API returned {(int)response.StatusCode}.";
            }
        }
        catch (JsonException)
        {
        }
        return $"API returned {(int)response.StatusCode} {response.ReasonPhrase}.";
    }
}
