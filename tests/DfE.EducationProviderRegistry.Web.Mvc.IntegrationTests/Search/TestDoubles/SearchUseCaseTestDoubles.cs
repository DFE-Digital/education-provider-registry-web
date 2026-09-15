using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;

internal static class SearchUseCaseTestDoubles
{
    internal static (StubSearchUseCase useCase, SearchResponse response) StubResponse()
    {
        SearchResponse response =
            SearchResponseTestDouble.Stub();

        UseCaseResponse<SearchResponse> stubbedResponse = UseCaseResponse<SearchResponse>.Success(response);

        StubSearchUseCase useCase = new(stubbedResponse);

        return (useCase, response);
    }

    internal static StubSearchUseCase StubFor(EstablishmentSearchResults searchResults, SearchFacets facets)
    {
        SearchResponse response =
            new(searchResults, facets, searchResults.Count);

        UseCaseResponse<SearchResponse> stubbedResponse =
            UseCaseResponse<SearchResponse>.Success(response);

        return new StubSearchUseCase(stubbedResponse);
    }
}

internal sealed class StubSearchUseCase
    : IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>
{
    private readonly UseCaseResponse<SearchResponse> _response;
    public SearchRequest? ReceivedRequest { get; private set; }

    public StubSearchUseCase(
        UseCaseResponse<SearchResponse> response)
    {
        _response = response;
    }

    public Task<UseCaseResponse<SearchResponse>> HandleRequestAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ReceivedRequest = request;
        return Task.FromResult(_response);
    }
}