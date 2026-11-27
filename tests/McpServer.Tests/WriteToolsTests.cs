using System.Net;
using System.Text.Json;
using ProductCatalog.McpServer.Tools;

namespace ProductCatalog.McpServer.Tests;

public class WriteToolsTests
{
    private static readonly CancellationToken None = CancellationToken.None;

    private static JsonElement Json(string text) => JsonDocument.Parse(text).RootElement;

    // ---------- create_product ----------

    [Fact]
    public async Task CreateProduct_InvalidInput_ListsEveryProblem_AndCallsNothing()
    {
        var handler = TestApi.WithCatalog();

        var result = await CatalogWriteTools.CreateProduct(TestApi.Client(handler), " ", 0m, 0, "Books", None);

        Assert.Equal("Product not created. Name is required. Price must be greater than 0. Stock must be greater than 0.", result);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task CreateProduct_UnknownCategory_IsRefused_WithoutPost()
    {
        var handler = TestApi.WithCatalog();

        var result = await CatalogWriteTools.CreateProduct(TestApi.Client(handler), "Toy Car", 10m, 5, "Toys", None);

        Assert.Equal("Product not created. Category 'Toys' does not exist. Available categories: 1 Electronics, 2 Books, 3 Clothing.", result);
        Assert.Empty(handler.Sent("POST", "api/products"));
    }

    [Fact]
    public async Task CreateProduct_DuplicateNameAnyCase_IsRefused_WithoutPost()
    {
        var handler = TestApi.WithCatalog();

        var result = await CatalogWriteTools.CreateProduct(TestApi.Client(handler), "  mcp test CABLE ", 10m, 5, "1", None);

        Assert.StartsWith("Product not created. 'MCP Test Cable' already exists (id 9, stock 25).", result);
        Assert.Empty(handler.Sent("POST", "api/products"));
    }

    [Theory]
    [InlineData("books", 2)]
    [InlineData("3", 3)]
    public async Task CreateProduct_Valid_PostsTrimmedNameAndCategoryId(string category, int expectedCategoryId)
    {
        var handler = TestApi.WithCatalog().Returns("POST", "api/products", HttpStatusCode.Created, new
        {
            productId = 10, productName = "USB-C Cable", price = 19.99m, stock = 5, categoryId = expectedCategoryId
        });

        var result = await CatalogWriteTools.CreateProduct(TestApi.Client(handler), "  USB-C Cable ", 19.99m, 5, category, None);

        Assert.StartsWith("Product created:", result);
        var body = Json(Assert.Single(handler.Sent("POST", "api/products")).Body!);
        Assert.Equal("USB-C Cable", body.GetProperty("productName").GetString());
        Assert.Equal(expectedCategoryId, body.GetProperty("categoryId").GetInt32());
        Assert.Equal(19.99m, body.GetProperty("price").GetDecimal());
        Assert.Equal(5, body.GetProperty("stock").GetInt32());

        var created = Json(result["Product created:".Length..]);
        Assert.Equal(10, created.GetProperty("productId").GetInt32());
    }

    [Fact]
    public async Task CreateProduct_ApiRefuses_ReturnsApiMessage()
    {
        var handler = TestApi.WithCatalog().Returns("POST", "api/products", HttpStatusCode.BadRequest, new
        {
            errors = new Dictionary<string, string[]> { ["Price"] = new[] { "Price is too high." } }
        });

        var result = await CatalogWriteTools.CreateProduct(TestApi.Client(handler), "Gold Cable", 99999m, 1, "Electronics", None);

        Assert.Equal("Product not created. The API said: Price is too high.", result);
    }

    // ---------- update_stock ----------

    private static FakeApiHandler WithProduct9() =>
        new FakeApiHandler().Ok("GET", "api/products/9", TestApi.Products().Single(p => p.ProductId == 9));

    [Fact]
    public async Task UpdateStock_NegativeStock_IsRefused_AndCallsNothing()
    {
        var handler = WithProduct9();

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, -1, None);

        Assert.Equal("Stock not changed. The new stock must be 0 or more.", result);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task UpdateStock_UnknownProduct_SaysNotFound()
    {
        var handler = new FakeApiHandler();

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 999, 5, None);

        Assert.Equal("Stock not changed. No product found with id 999.", result);
        Assert.Empty(handler.Sent("PUT", "api/products/999"));
    }

    [Fact]
    public async Task UpdateStock_SameStock_ChangesNothing_AndSendsNoPut()
    {
        var handler = WithProduct9();

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, 25, None);

        Assert.Equal("MCP Test Cable already has stock 25. Nothing changed.", result);
        Assert.Empty(handler.Sent("PUT", "api/products/9"));
    }

    [Fact]
    public async Task UpdateStock_Valid_SendsPut_AndReportsOldAndNew()
    {
        var handler = WithProduct9().Returns("PUT", "api/products/9", HttpStatusCode.NoContent);

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, 30, None);

        Assert.Equal("Stock updated: MCP Test Cable (id 9) 25 -> 30.", result);
        var body = Json(Assert.Single(handler.Sent("PUT", "api/products/9")).Body!);
        Assert.Equal(30, body.GetProperty("stock").GetInt32());
        Assert.Equal("MCP Test Cable", body.GetProperty("productName").GetString());
    }

    [Fact]
    public async Task UpdateStock_BelowTwenty_AddsLowStockNote()
    {
        var handler = WithProduct9().Returns("PUT", "api/products/9", HttpStatusCode.NoContent);

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, 5, None);

        Assert.Equal("Stock updated: MCP Test Cable (id 9) 25 -> 5. It is still below 20 (low stock).", result);
    }

    [Fact]
    public async Task UpdateStock_ApiRefuses_ReturnsApiMessage()
    {
        var handler = WithProduct9().Returns("PUT", "api/products/9", HttpStatusCode.BadRequest, new { title = "Stock is locked." });

        var result = await CatalogWriteTools.UpdateStock(TestApi.Client(handler), 9, 30, None);

        Assert.Equal("Stock not changed. The API said: Stock is locked.", result);
    }

    // ---------- low_stock_report ----------

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task LowStockReport_InvalidThreshold_IsRefused(int threshold)
    {
        var result = await CatalogWriteTools.LowStockReport(TestApi.Client(TestApi.WithCatalog()), threshold, None);

        Assert.Equal("Threshold must be greater than 0.", result);
    }

    [Fact]
    public async Task LowStockReport_Default20_ListsLowestFirst_WithCountPerCategory()
    {
        var result = Json(await CatalogWriteTools.LowStockReport(TestApi.Client(TestApi.WithCatalog())));

        Assert.Equal(20, result.GetProperty("threshold").GetInt32());
        Assert.Equal(2, result.GetProperty("count").GetInt32());
        Assert.Equal(new[] { "Mechanical Keyboard", "Product3" },
            result.GetProperty("products").EnumerateArray().Select(p => p.GetProperty("productName").GetString()));
        var byCategory = Assert.Single(result.GetProperty("byCategory").EnumerateArray());
        Assert.Equal("Electronics", byCategory.GetProperty("category").GetString());
        Assert.Equal(2, byCategory.GetProperty("count").GetInt32());
    }

    [Fact]
    public async Task LowStockReport_CustomThreshold_UsesIt()
    {
        var result = Json(await CatalogWriteTools.LowStockReport(TestApi.Client(TestApi.WithCatalog()), 30, None));

        Assert.Equal(3, result.GetProperty("count").GetInt32());
    }

    [Fact]
    public async Task LowStockReport_NothingBelowThreshold_SaysSo()
    {
        var result = await CatalogWriteTools.LowStockReport(TestApi.Client(TestApi.WithCatalog()), 10, None);

        Assert.Equal("No products with stock below 10.", result);
    }
}
