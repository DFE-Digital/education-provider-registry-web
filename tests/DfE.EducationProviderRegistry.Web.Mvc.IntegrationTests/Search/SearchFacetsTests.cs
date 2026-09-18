using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchFacetsTests
{
    [Fact]
    public async Task Displays_Facets_Returned_By_SearchResponse()
    {
        // Arrange
        SearchFacets responseFacets =
            SearchFacetsTestDouble.Stub();

        StubSearchUseCase useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults: SearchAggregateResults.CreateEmpty(),
                responseFacets);

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .Build();

        // Act
        using HttpResponseMessage response =
            await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        AssertFiltersDisplay(document, responseFacets, selections: null);
    }

    [Fact]
    public async Task Displays_Selected_Facets_From_SearchRequest()
    {
        // Arrange
        SearchFacets responseFacets =
            SearchFacetsTestDouble.Stub();

        StubSearchUseCase useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults: SearchAggregateResults.CreateEmpty(),
                responseFacets);

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("facet1", ["value-2"])
                .WithFilter("facet2", ["value-4"])
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        Dictionary<string, List<string>> expectedSelections = new()
        {
            ["facet1"] = ["value-2"],
            ["facet2"] = ["value-4"]
        };

        AssertFiltersDisplay(document, responseFacets, expectedSelections);
    }

    private static void AssertFiltersDisplay(IHtmlDocument document, SearchFacets facets, Dictionary<string, List<string>>? selections = null)
    {
        SearchFiltersComponent filterComponent = new(document);

        Dictionary<string, List<string>> facetSelections = selections ?? [];

        List<Filter> filters = filterComponent.GetFilters();

        Assert.Equal(facets.Facets.Count, filters.Count);

        foreach (Filter filter in filters)
        {
            SearchFacet expectedFacet =
                facets.Facets.Single((facet) =>
                    facet.Name == filter.Name);

            foreach (FilterValue value in filter.FilterValues)
            {
                FacetResult expectedResult =
                    expectedFacet.Results.Single(result =>
                        result.Value == value.Value);

                Assert.StartsWith(expectedResult.Label, value.Label);
                Assert.EndsWith($"({expectedResult.Count})", value.Label);
                Assert.Equal(expectedResult.Value, value.Value);

                bool shouldBeSelected =
                    facetSelections.TryGetValue(filter.Name, out List<string>? selectedValues)
                        && selectedValues.Contains(value.Value);

                Assert.Equal(shouldBeSelected, value.Selected);
            }
        }
    }
}
