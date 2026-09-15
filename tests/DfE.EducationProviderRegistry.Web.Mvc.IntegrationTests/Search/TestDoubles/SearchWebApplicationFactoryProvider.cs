using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;

internal static class SearchWebApplicationFactoryProvider
{
    public static WebApplicationFactory<Program> CreateFactory(IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> stubUseCase)
    {
        return new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<
                            IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>>();

                        services.AddSingleton(_ => stubUseCase);
                    }));
    }
}
