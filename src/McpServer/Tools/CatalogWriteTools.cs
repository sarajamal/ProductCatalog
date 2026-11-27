using System.ComponentModel;
using ModelContextProtocol.Server;
using ProductCatalog.McpServer.Models;
using static ProductCatalog.McpServer.Tools.CatalogReadTools;

namespace ProductCatalog.McpServer.Tools;

/// <summary>
/// Phase 4 tools. create_product and update_stock change data through the API
/// (Claude Desktop asks the user to approve each call). low_stock_report only reads.
/// </summary>
[McpServerToolType]
public static class CatalogWriteTools
{
    // Same rule as the Angular client: stock under 20 shows in red.
    private const int DefaultLowStockThreshold = 20;

    [McpServerTool(Name = "create_product", Destructive = false, Idempotent = false, ReadOnly = false),
     Description("Creates a new product in the catalog. CHANGES DATA. Price and stock must be greater than 0. " +
                 "Category can be an id or a name, for example 2 or Books. Refuses a name that already exists.")]
    public static Task<string> CreateProduct(
        ProductCatalogClient api,
        [Description("Product name, for example 'USB-C Cable'.")] string name,
        [Description("Price, greater than 0, for example 19.99.")] decimal price,
        [Description("Starting stock, greater than 0.")] int stock,
        [Description("Category id or name, for example 1 or Electronics.")] string category,
        CancellationToken ct) =>
        Run(api, async () =>
        {
            var problems = new List<string>();
            if (string.IsNullOrWhiteSpace(name)) problems.Add("Name is required.");
            if (price <= 0) problems.Add("Price must be greater than 0.");
            if (stock <= 0) problems.Add("Stock must be greater than 0.");
            if (problems.Count > 0) return "Product not created. " + string.Join(" ", problems);

            var categories = await api.GetCategoriesAsync(ct);
            var match = FindCategory(categories, category);
            if (match is null)
            {
                var available = string.Join(", ", categories.Select(c => $"{c.CategoryId} {c.CategoryName}"));
                return $"Product not created. Category '{category}' does not exist. Available categories: {available}.";
            }

            var products = await api.GetProductsAsync(ct);
            var existing = products.FirstOrDefault(p => string.Equals(p.ProductName.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing is not null)
            {
                return $"Product not created. '{existing.ProductName}' already exists (id {existing.ProductId}, stock {existing.Stock}). " +
                       "Use update_stock to change its stock.";
            }

            var (created, error) = await api.CreateProductAsync(new Product
            {
                ProductName = name.Trim(),
                Price = price,
                Stock = stock,
                CategoryId = match.CategoryId
            }, ct);

            if (created is null) return "Product not created. The API said: " + error;
            created.Category ??= match;
            return "Product created:\n" + Serialize(ToView(created));
        });

    [McpServerTool(Name = "update_stock", Destructive = false, Idempotent = true, ReadOnly = false),
     Description("Sets the stock of one product to a new number. CHANGES DATA. Give the product id " +
                 "(use search_products first if you only know the name). Stock must be 0 or more.")]
    public static Task<string> UpdateStock(
        ProductCatalogClient api,
        [Description("The product id, for example 2.")] int productId,
        [Description("The new stock number (not an amount to add), 0 or more.")] int newStock,
        CancellationToken ct) =>
        Run(api, async () =>
        {
            if (newStock < 0) return "Stock not changed. The new stock must be 0 or more.";

            var product = await api.GetProductAsync(productId, ct);
            if (product is null) return $"Stock not changed. No product found with id {productId}.";

            var oldStock = product.Stock;
            if (oldStock == newStock) return $"{product.ProductName} already has stock {newStock}. Nothing changed.";

            product.Stock = newStock;
            var error = await api.UpdateProductAsync(product, ct);
            if (error is not null) return "Stock not changed. The API said: " + error;

            var note = newStock < DefaultLowStockThreshold ? $" It is still below {DefaultLowStockThreshold} (low stock)." : "";
            return $"Stock updated: {product.ProductName} (id {productId}) {oldStock} -> {newStock}.{note}";
        });

    [McpServerTool(Name = "low_stock_report", Destructive = false, Idempotent = true, ReadOnly = true),
     Description("Lists products whose stock is below a threshold (default 20, the same rule the web app uses for the red alert), " +
                 "lowest first, with a count per category.")]
    public static Task<string> LowStockReport(
        ProductCatalogClient api,
        [Description("Show products with stock below this number. Default 20.")] int threshold = DefaultLowStockThreshold,
        CancellationToken ct = default) =>
        Run(api, async () =>
        {
            if (threshold <= 0) return "Threshold must be greater than 0.";

            var low = (await api.GetProductsAsync(ct))
                .Where(p => p.Stock < threshold)
                .OrderBy(p => p.Stock)
                .ToList();

            if (low.Count == 0) return $"No products with stock below {threshold}.";

            return Serialize(new
            {
                Threshold = threshold,
                Count = low.Count,
                ByCategory = low.GroupBy(p => p.Category?.CategoryName ?? $"#{p.CategoryId}")
                                .Select(g => new { Category = g.Key, Count = g.Count() }),
                Products = low.Select(ToView)
            });
        });

    private static Category? FindCategory(IReadOnlyList<Category> categories, string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return null;
        var text = category.Trim();
        return int.TryParse(text, out var id)
            ? categories.FirstOrDefault(c => c.CategoryId == id)
            : categories.FirstOrDefault(c => string.Equals(c.CategoryName, text, StringComparison.OrdinalIgnoreCase));
    }
}
