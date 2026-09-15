using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchSortTests
{
    [Theory]
    [InlineData("az", "asc")]
    [InlineData("za", "desc")]
    [InlineData("invalid", "asc")]
    public async Task Maps_Sort_Direction_To_Search_Request(string sort, string expectedDirection)
    {
        // Arrange
        StubSearchUseCase useCase =
            SearchUseCaseTestDoubles.StubFor(
                EstablishmentSearchResultsTestDouble.EmptyStub(),
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithSortDirection(sort)
                .Build();

        // Act
        await client.SendAsync(
            message,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(useCase.ReceivedRequest);

        Assert.Equal(
            expectedDirection,
            useCase.ReceivedRequest.SortOrder.Direction.Value);
    }

    [Theory]
    [InlineData("az")]
    [InlineData("za")]
    public async Task Displays_Selected_Sort_Direction(string sortDirection)
    {
        // Arrange
        (IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase,
            SearchResponse _) = SearchUseCaseTestDoubles.StubResponse();

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("School")
                .WithSortDirection(sortDirection)
                .Build();

        // Act
        using HttpResponseMessage response =
            await client.SendAsync(
                message,
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchSortComponent sortComponent = new(document);

        Assert.Equal(sortDirection, sortComponent.GetSelectedSortDirection());
    }
}