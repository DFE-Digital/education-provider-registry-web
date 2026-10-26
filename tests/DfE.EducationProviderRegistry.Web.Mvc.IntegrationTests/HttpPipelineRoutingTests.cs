using System.Net;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public sealed class HttpPipelineRoutingTests
{
    [Fact]
    public async Task Unknown_Route_Returns_404()
    {
        // Arrange
        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .UseProduction();

        using HttpClient client = factory.CreateClient();
        // Act
        HttpResponseMessage response = await client.GetAsync("/unknown", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Unhandled_DependencyException_Returns_500()
    {
        // Arrange
        ArgumentException ex = new("dep-error");
        ThrowingUseCaseStub<SearchRequest, SearchResponse> useCase = new(ex);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider
                .StubUseCase(useCase)
                .UseProduction();

        using HttpClient client = factory.CreateClient();

        // Act
        HttpResponseMessage response =
            await client.GetAsync(SearchRoutes.SearchResults(), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        string content =
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("An error occurred while processing your request.", content);
    }
}
