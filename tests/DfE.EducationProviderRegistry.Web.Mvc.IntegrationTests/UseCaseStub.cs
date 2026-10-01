using DfE.Core.Libraries.CleanArchitecture.Application;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

internal sealed class UseCaseStub<TRequest, TResponseModel> : IUseCase<TRequest, UseCaseResponse<TResponseModel>>
    where TRequest : IUseCaseRequest<UseCaseResponse<TResponseModel>>
    where TResponseModel : class
{
    private readonly UseCaseResponse<TResponseModel> _response;
    public TRequest? ReceivedRequest { get; private set; }

    public UseCaseStub(
        UseCaseResponse<TResponseModel> response)
    {
        _response = response;
    }

    public Task<UseCaseResponse<TResponseModel>> HandleRequestAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        ReceivedRequest = request;
        return Task.FromResult(_response);
    }
}
