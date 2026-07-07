using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests;

public class IntegrationTestExample
{
    private readonly CancellationToken _ct;
    public IntegrationTestExample()
    {
        _ct = TestContext.Current.CancellationToken;
    }

    [Fact]
    public async Task ExampleIntegrationTest()
    {
        using WebApplicationFactory<Program> factory = new();
        using HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/", _ct);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
