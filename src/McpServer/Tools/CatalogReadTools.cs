using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ProductCatalog.McpServer.Models;

namespace ProductCatalog.McpServer.Tools;

/// <summary>
/// Read-only MCP tools (phase 2). Each tool calls ProductCatalogApi and returns JSON text.
/// Errors are returned as readable text instead of exceptions, so Claude can explain them.
/// </summary>
[McpServerToolType]
public static class CatalogReadTools
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    [McpServerTool(Name = "list_categories"), Description("Lists all product categories (id and name) from the ProductCatalog API.")]
    public static Task<string> ListCategories(ProductCatalogClient api, CancellationToken ct) =>
        Run(api, async () => Serialize(await api.GetCategoriesAsync(ct)));

    [McpServerTool(Name = "list_products"), Description("Lists all products with their category, price and stock, sorted by stock (lowest first).")]
    public static Task<string> ListProducts(ProductCatalogClient api, CancellationToken ct) =>
        Run(api, async () =>
        {
            var products = await api.GetProductsAsync(ct);
            return Serialize(products.OrderBy(p => p.Stock).Select(ToView));
        });

    [McpServerTool(Name = "get_product"), Description("Gets one product by its id. Returns a 'not found' message if the id does not exist.")]
    public static Task<string> GetProduct(
        ProductCatalogClient api,
        [Description("The product id, for example 3.")] int productId,
        CancellationToken ct) =>
        Run(api, async () =>
        {
            var product = await api.GetProductAsync(productId, ct);
            return product is null
                ? $"No product found with id {productId}."
                : Serialize(ToView(product));
        });

    [McpServerTool(Name = "get_products_by_category"), Description("Lists the products in one category, by category id. Use list_categories to find the id.")]
    public static Task<string> GetProductsByCategory(
        ProductCatalogClient api,
        [Description("The category id, for example 1.")] int categoryId,
        CancellationToken ct) =>
        Run(api, async () =>
        {
            var products = (await api.GetProductsAsync(ct)).Where(p => p.CategoryId == categoryId).ToList();
            return products.Count == 0
                ? $"No products found in category {categoryId}."
                : Serialize(products.OrderBy(p => p.Stock).Select(ToView));
        });

    [McpServerTool(Name = "search_products"), Description("Searches products whose name contains the given text (case-insensitive).")]
    public static Task<string> SearchProducts(
        ProductCatalogClient api,
        [Description("Text to search for in the product name, for example 'key'.")] string query,
        CancellationToken ct) =>
        Run(api, async () =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Please give some text to search for.";
            }
            var products = (await api.GetProductsAsync(ct))
                .Where(p => p.ProductName.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
            return products.Count == 0
                ? $"No products found matching '{query}'."
                : Serialize(products.OrderBy(p => p.Stock).Select(ToView));
        });

    // ---- helpers ----

    private static object ToView(Product p) => new
    {
        p.ProductId,
        p.ProductName,
        Category = p.Category?.CategoryName ?? $"#{p.CategoryId}",
        p.CategoryId,
        p.Price,
        p.Stock
    };

    private static string Serialize(object value) => JsonSerializer.Serialize(value, Json);

    private static async Task<string> Run(ProductCatalogClient api, Func<Task<string>> action)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException ex)
        {
            return $"Could not reach ProductCatalogApi at {api.BaseAddress} ({ex.Message}). " +
                   "Make sure the API is running (scripts\\start-api.bat).";
        }
        catch (TaskCanceledException)
        {
            return $"ProductCatalogApi at {api.BaseAddress} did not answer in time.";
        }
    }
}
