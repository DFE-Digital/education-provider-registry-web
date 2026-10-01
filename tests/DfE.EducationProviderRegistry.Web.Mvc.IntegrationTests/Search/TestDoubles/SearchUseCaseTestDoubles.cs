using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;

internal static class SearchUseCaseTestDoubles
{
    internal static (UseCaseStub<SearchRequest, SearchResponse> useCase, SearchResponse response) StubResponse()
    {
        SearchResults<SearchAggregateResults, SearchFacets> result = SearchResultsTestDouble.Stub();

        SearchResponse response =
            new(result.Results!, result.FacetResults, result.TotalCount);

        UseCaseResponse<SearchResponse> stubbedResponse = UseCaseResponse<SearchResponse>.Success(response);

        UseCaseStub<SearchRequest, SearchResponse> useCase = new(stubbedResponse);

        return (useCase, response);
    }

    internal static UseCaseStub<SearchRequest, SearchResponse> StubFor(SearchAggregateResults searchResults, SearchFacets facets)
    {
        SearchResponse response =
            new(searchResults, facets, searchResults.Count);

        UseCaseResponse<SearchResponse> stubbedResponse =
            UseCaseResponse<SearchResponse>.Success(response);

        return new UseCaseStub<SearchRequest, SearchResponse>(stubbedResponse);
    }
}
