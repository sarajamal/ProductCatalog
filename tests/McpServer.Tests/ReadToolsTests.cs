using System.Text.Json;
using ProductCatalog.McpServer.Tools;

namespace ProductCatalog.McpServer.Tests;

public class ReadToolsTests
{
    private static readonly CancellationToken None = CancellationToken.None;
    private readonly ProductCatalogClient _api = TestApi.Client(TestApi.WithCatalog());

    private static JsonElement Json(string text) => JsonDocument.Parse(text).RootElement;

    [Fact]
    public async Task ListCategories_ReturnsAllCategories()
    {
        var result = Json(await CatalogReadTools.ListCategories(_api, None));

        Assert.Equal(new[] { "Electronics", "Books", "Clothing" },
            result.EnumerateArray().Select(c => c.GetProperty("categoryName").GetString()));
    }

    [Fact]
    public async Task ListProducts_SortsByStock_LowestFirst()
    {
        var result = Json(await CatalogReadTools.ListProducts(_api, None));

        Assert.Equal(new[] { 16, 19, 25, 65, 97 }, result.EnumerateArray().Select(p => p.GetProperty("stock").GetInt32()));
        Assert.Equal("Electronics", result[0].GetProperty("category").GetString());
    }

    [Fact]
    public async Task GetProduct_ReturnsProduct_WhenItExists()
    {
        var api = TestApi.Client(new FakeApiHandler().Ok("GET", "api/products/9", TestApi.Products().Single(p => p.ProductId == 9)));

        var result = Json(await CatalogReadTools.GetProduct(api, 9, None));

        Assert.Equal("MCP Test Cable", result.GetProperty("productName").GetString());
    }

    [Fact]
    public async Task GetProduct_SaysNotFound_WhenIdDoesNotExist()
    {
        var result = await CatalogReadTools.GetProduct(_api, 999, None);

        Assert.Equal("No product found with id 999.", result);
    }

    [Fact]
    public async Task GetProductsByCategory_Empty_ReturnsEveryCategoryWithItsProducts()
    {
        var result = Json(await CatalogReadTools.GetProductsByCategory(_api, null, None));

        Assert.Equal(3, result.GetArrayLength());
        var electronics = result[0].GetProperty("products");
        Assert.Equal(new[] { 16, 19, 25, 97 }, electronics.EnumerateArray().Select(p => p.GetProperty("stock").GetInt32()));
        Assert.Equal(0, result[2].GetProperty("products").GetArrayLength());
    }

    [Theory]
    [InlineData("books")]
    [InlineData("BOOKS")]
    [InlineData("2")]
    [InlineData(" Books ")]
    public async Task GetProductsByCategory_FindsCategoryByNameAnyCaseOrById(string category)
    {
        var result = Json(await CatalogReadTools.GetProductsByCategory(_api, category, None));

        Assert.Equal("Books", result.GetProperty("categoryName").GetString());
        Assert.Equal("Clean Code Book", Assert.Single(result.GetProperty("products").EnumerateArray()).GetProperty("productName").GetString());
    }

    [Fact]
    public async Task GetProductsByCategory_UnknownCategory_ListsAvailableCategories()
    {
        var result = await CatalogReadTools.GetProductsByCategory(_api, "4", None);

        Assert.Equal("Category '4' does not exist. Available categories: 1 Electronics, 2 Books, 3 Clothing.", result);
    }

    [Fact]
    public async Task GetProductsByCategory_CategoryWithoutProducts_SaysSo()
    {
        var result = await CatalogReadTools.GetProductsByCategory(_api, "Clothing", None);

        Assert.Equal("Category 3 Clothing exists but has no products.", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchProducts_BlankQuery_AsksForText(string query)
    {
        var result = await CatalogReadTools.SearchProducts(_api, query, None);

        Assert.Equal("Please give some text to search for.", result);
    }

    [Fact]
    public async Task SearchProducts_IgnoresCase()
    {
        var result = Json(await CatalogReadTools.SearchProducts(_api, "KEY", None));

        Assert.Equal("Mechanical Keyboard", Assert.Single(result.EnumerateArray()).GetProperty("productName").GetString());
    }

    [Fact]
    public async Task SearchProducts_NoMatch_SaysSo()
    {
        var result = await CatalogReadTools.SearchProducts(_api, "zzz", None);

        Assert.Equal("No products found matching 'zzz'.", result);
    }
}
