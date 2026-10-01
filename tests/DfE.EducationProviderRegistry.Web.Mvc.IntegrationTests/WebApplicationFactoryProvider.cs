using DfE.Core.Libraries.CleanArchitecture.Application;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

internal static class WebApplicationFactoryProvider
{
    public static WebApplicationFactory<Program> CreateFactory<TRequest, TResponse>(IUseCase<TRequest, UseCaseResponse<TResponse>> stubUseCase)
        where TRequest : IUseCaseRequest<UseCaseResponse<TResponse>>
        where TResponse : class
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<
                        IUseCase<TRequest, UseCaseResponse<TResponse>>>();

                    services.AddSingleton<IUseCase<TRequest, UseCaseResponse<TResponse>>>(_ => stubUseCase);
                }));
    }
}
