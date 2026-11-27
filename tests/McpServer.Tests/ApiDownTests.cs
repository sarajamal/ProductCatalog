using System.Net;
using ProductCatalog.McpServer.Tools;

namespace ProductCatalog.McpServer.Tests;

/// <summary>Every tool must return a readable message instead of throwing when the API is not there.</summary>
public class ApiDownTests
{
    private static readonly CancellationToken None = CancellationToken.None;

    private static ProductCatalogClient DownApi() =>
        TestApi.Client(new FakeApiHandler { ThrowOnSend = new HttpRequestException("No connection could be made") });

    public static IEnumerable<object[]> AllTools() => new[]
    {
        new object[] { "list_categories", (Func<ProductCatalogClient, Task<string>>)(api => CatalogReadTools.ListCategories(api, None)) },
        new object[] { "list_products", (Func<ProductCatalogClient, Task<string>>)(api => CatalogReadTools.ListProducts(api, None)) },
        new object[] { "get_product", (Func<ProductCatalogClient, Task<string>>)(api => CatalogReadTools.GetProduct(api, 1, None)) },
        new object[] { "get_products_by_category", (Func<ProductCatalogClient, Task<string>>)(api => CatalogReadTools.GetProductsByCategory(api, "1", None)) },
        new object[] { "search_products", (Func<ProductCatalogClient, Task<string>>)(api => CatalogReadTools.SearchProducts(api, "key", None)) },
        new object[] { "create_product", (Func<ProductCatalogClient, Task<string>>)(api => CatalogWriteTools.CreateProduct(api, "Cable", 1m, 1, "1", None)) },
        new object[] { "update_stock", (Func<ProductCatalogClient, Task<string>>)(api => CatalogWriteTools.UpdateStock(api, 9, 30, None)) },
        new object[] { "low_stock_report", (Func<ProductCatalogClient, Task<string>>)(api => CatalogWriteTools.LowStockReport(api, 20, None)) }
    };

    [Theory]
    [MemberData(nameof(AllTools))]
    public async Task ApiDown_EveryTool_SaysHowToStartTheApi(string tool, Func<ProductCatalogClient, Task<string>> call)
    {
        var result = await call(DownApi());

        Assert.True(result.StartsWith($"Could not reach ProductCatalogApi at {TestApi.BaseUrl}"), $"{tool}: {result}");
        Assert.Contains(@"scripts\start-api.bat", result);
    }

    [Fact]
    public async Task ApiTooSlow_SaysItDidNotAnswerInTime()
    {
        var api = TestApi.Client(new FakeApiHandler { ThrowOnSend = new TaskCanceledException("timeout") });

        var result = await CatalogReadTools.ListProducts(api, None);

        Assert.Equal($"ProductCatalogApi at {TestApi.BaseUrl} did not answer in time.", result);
    }

    /// <summary>
    /// Documents current behaviour (McpServer GAP-006): a 500 from a running API is reported
    /// as "could not reach the API". When GAP-006 is fixed, change this test to the new message.
    /// </summary>
    [Fact]
    public async Task ApiServerError500_IsReportedAsUnreachable_KnownGap006()
    {
        var api = TestApi.Client(new FakeApiHandler().Returns("GET", "api/products", HttpStatusCode.InternalServerError));

        var result = await CatalogReadTools.ListProducts(api, None);

        Assert.StartsWith("Could not reach ProductCatalogApi", result);
    }
}
