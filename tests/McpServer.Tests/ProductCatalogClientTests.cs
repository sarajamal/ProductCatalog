using System.Net;
using System.Text.Json;
using ProductCatalog.McpServer.Models;

namespace ProductCatalog.McpServer.Tests;

public class ProductCatalogClientTests
{
    [Fact]
    public async Task GetProductAsync_ReturnsProduct_WhenApiAnswers200()
    {
        var handler = new FakeApiHandler().Ok("GET", "api/products/9", TestApi.Products().Single(p => p.ProductId == 9));

        var product = await TestApi.Client(handler).GetProductAsync(9);

        Assert.NotNull(product);
        Assert.Equal("MCP Test Cable", product!.ProductName);
        Assert.Equal(25, product.Stock);
    }

    [Fact]
    public async Task GetProductAsync_ReturnsNull_WhenApiAnswers404()
    {
        var product = await TestApi.Client(new FakeApiHandler()).GetProductAsync(99);

        Assert.Null(product);
    }

    [Fact]
    public async Task CreateProductAsync_TurnsValidationProblemIntoOneLine_WhenApiAnswers400()
    {
        var handler = new FakeApiHandler().Returns("POST", "api/products", HttpStatusCode.BadRequest, new
        {
            title = "One or more validation errors occurred.",
            errors = new Dictionary<string, string[]>
            {
                ["Price"] = new[] { "Price must be greater than 0." },
                ["ProductName"] = new[] { "Name is required." }
            }
        });

        var (created, error) = await TestApi.Client(handler).CreateProductAsync(new Product { ProductName = "", Price = 0 });

        Assert.Null(created);
        Assert.Equal("Price must be greater than 0. Name is required.", error);
    }

    [Fact]
    public async Task UpdateProductAsync_ReturnsProblemTitle_WhenApiAnswers404WithTitle()
    {
        var handler = new FakeApiHandler().Returns("PUT", "api/products/9", HttpStatusCode.NotFound, new { title = "Not Found" });

        var error = await TestApi.Client(handler).UpdateProductAsync(new Product { ProductId = 9 });

        Assert.Equal("Not Found", error);
    }

    [Fact]
    public async Task UpdateProductAsync_ReturnsStatusText_WhenErrorBodyIsNotJson()
    {
        var handler = new FakeApiHandler().Returns("PUT", "api/products/9", HttpStatusCode.BadRequest, "bad input");

        var error = await TestApi.Client(handler).UpdateProductAsync(new Product { ProductId = 9 });

        Assert.Equal("API returned 400 Bad Request.", error);
    }

    [Fact]
    public async Task UpdateProductAsync_ReturnsNull_AndSendsProductWithoutCategoryObject()
    {
        var handler = new FakeApiHandler().Returns("PUT", "api/products/9", HttpStatusCode.NoContent);
        var product = TestApi.Products().Single(p => p.ProductId == 9);
        product.Stock = 30;

        var error = await TestApi.Client(handler).UpdateProductAsync(product);

        Assert.Null(error);
        var body = JsonDocument.Parse(Assert.Single(handler.Sent("PUT", "api/products/9")).Body!).RootElement;
        Assert.Equal(30, body.GetProperty("stock").GetInt32());
        Assert.Equal(1, body.GetProperty("categoryId").GetInt32());
        Assert.True(!body.TryGetProperty("category", out var category) || category.ValueKind == JsonValueKind.Null);
    }
}
