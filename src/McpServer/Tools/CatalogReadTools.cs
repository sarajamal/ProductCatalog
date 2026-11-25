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

    [McpServerTool(Name = "get_products_by_category"),
     Description("Lists products by category. Give a category id or name (for example 1 or Books) to get one category, " +
                 "or leave it empty to get every category with its products.")]
    public static Task<string> GetProductsByCategory(
        ProductCatalogClient api,
        [Description("Category id or name, for example 1 or Books. Leave empty for all categories.")] string? category = null,
        CancellationToken ct = default) =>
        Run(api, async () =>
        {
            var categories = await api.GetCategoriesAsync(ct);
            var products = await api.GetProductsAsync(ct);

            // No category given: every category with its products
            if (string.IsNullOrWhiteSpace(category))
            {
                return Serialize(categories.Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    Products = products.Where(p => p.CategoryId == c.CategoryId).OrderBy(p => p.Stock).Select(ToView)
                }));
            }

            // One category, by id or by name
            var text = category.Trim();
            var match = int.TryParse(text, out var id)
                ? categories.FirstOrDefault(c => c.CategoryId == id)
                : categories.FirstOrDefault(c => string.Equals(c.CategoryName, text, StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                var available = string.Join(", ", categories.Select(c => $"{c.CategoryId} {c.CategoryName}"));
                return $"Category '{text}' does not exist. Available categories: {available}.";
            }

            var inCategory = products.Where(p => p.CategoryId == match.CategoryId).OrderBy(p => p.Stock).Select(ToView).ToList();
            return inCategory.Count == 0
                ? $"Category {match.CategoryId} {match.CategoryName} exists but has no products."
                : Serialize(new { match.CategoryId, match.CategoryName, Products = inCategory });
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

    internal static object ToView(Product p) => new
    {
        p.ProductId,
        p.ProductName,
        Category = p.Category?.CategoryName ?? $"#{p.CategoryId}",
        p.CategoryId,
        p.Price,
        p.Stock
    };

    internal static string Serialize(object value) => JsonSerializer.Serialize(value, Json);

    internal static async Task<string> Run(ProductCatalogClient api, Func<Task<string>> action)
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
