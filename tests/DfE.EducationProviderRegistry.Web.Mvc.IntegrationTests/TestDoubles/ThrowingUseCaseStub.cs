using DfE.Core.Libraries.CleanArchitecture.Application;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;

public sealed class ThrowingUseCaseStub<TRequest, TResponseModel> : IUseCase<TRequest, UseCaseResponse<TResponseModel>>
    where TRequest : IUseCaseRequest<UseCaseResponse<TResponseModel>>
    where TResponseModel : class
{
    private readonly Exception _exception;

    public ThrowingUseCaseStub(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        _exception = exception;
    }

    public Task<UseCaseResponse<TResponseModel>> HandleRequestAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        throw _exception;
    }
}
