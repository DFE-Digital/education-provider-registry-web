using DfE.Core.Libraries.CleanArchitecture.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.WebApplicationFactory.Extensions;

public static class WebApplicationFactoryProvider
{
    public static WebApplicationFactory<Program> Default =>
        new WebApplicationFactory<Program>()
            .UseProduction()
            .UseClientOptions();

    public static WebApplicationFactory<Program> WithConfiguration(
        IReadOnlyDictionary<string, string?> configuration)
    {
        return Default.WithWebHostBuilder((builder) =>
        {
            builder.ConfigureAppConfiguration((services, configurationBuilder) =>
            {
                configurationBuilder.Sources.Clear();
                configurationBuilder.AddInMemoryCollection(configuration);
            });
        });
    }

    public static WebApplicationFactory<Program> StubUseCase<TRequest, TResponse>(IUseCase<TRequest, UseCaseResponse<TResponse>> stubUseCase)
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
