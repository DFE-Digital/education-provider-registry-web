using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.Testing;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;

internal static class IUseCaseTestDoubles
{
    internal static IUseCase<TRequest, TResponse> Default<TRequest, TResponse>()
        where TRequest : IUseCaseRequest<TResponse>
        where TResponse : class
    {
        return MockTestDouble.Default<IUseCase<TRequest, TResponse>>().Object;
    }

    internal static Spy<TRequest, TModel> WithSpy<TRequest, TModel>(UseCaseResponse<TModel> response)
        where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
        where TModel : class
    {
        return new Spy<TRequest, TModel>(response);
    }

    internal static IUseCase<TRequest, UseCaseResponse<TModel>> WithResponse<TRequest, TModel>(
        UseCaseResponse<TModel> response)
        where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
        where TModel : class
    {
        Mock<IUseCase<TRequest, UseCaseResponse<TModel>>> mock = new();

        mock
            .Setup(x => x.HandleRequestAsync(It.IsAny<TRequest>()))
            .ReturnsAsync(response);

        return mock.Object;
    }

    internal sealed class Spy<TRequest, TModel>
        where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
        where TModel : class
    {
        private readonly Mock<IUseCase<TRequest, UseCaseResponse<TModel>>> _mock;

        private readonly List<TRequest> _capturedRequests;

        internal Spy(UseCaseResponse<TModel> response)
        {
            _mock = new();

            _capturedRequests = [];

            _mock
                .Setup(x => x.HandleRequestAsync(It.IsAny<TRequest>()))
                .Callback<TRequest>(request => _capturedRequests.Add(request))
                .ReturnsAsync(response);
        }

        public IUseCase<TRequest, UseCaseResponse<TModel>> Object => _mock.Object;

        public IReadOnlyList<TRequest> CapturedRequests => _capturedRequests;

        public void VerifyCalled(int count)
        {
            _mock.Verify(
                x => x.HandleRequestAsync(It.IsAny<TRequest>()),
                Times.Exactly(count));
        }
    }
}