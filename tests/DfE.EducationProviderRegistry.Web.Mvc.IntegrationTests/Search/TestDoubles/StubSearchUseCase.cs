using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;

internal sealed class StubSearchUseCase : IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>
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