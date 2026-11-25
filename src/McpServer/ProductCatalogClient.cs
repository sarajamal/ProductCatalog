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
}
