using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbBehaviourIntegrationTests
{
    [Fact]
    public async Task Test()
    {
        using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .UseClientOptions()
                .UseProduction()
                .UseBreadcrumbTestMvcInfrastructure();

        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/breadcrumb-tests/redirect", TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }
}

internal static class BreadcrumbTestDependencies
{
    public static WebApplicationFactory<Program> UseBreadcrumbTestMvcInfrastructure(this WebApplicationFactory<Program> factory)
    {
        return factory
            .WithWebHostBuilder((builder) =>
                builder.ConfigureTestServices((services) =>
                {
                    services.RemoveAll<IActionResultExecutor<ViewResult>>();

                    services.AddSingleton<
                        IActionResultExecutor<ViewResult>,
                        NoOpViewResultExecutor>();
                }).
                ConfigureServices((services) =>
                {
                    ServiceDescriptor descriptor = services.Single(
                        (descriptor) =>
                            descriptor.ServiceType == typeof(ApplicationPartManager));

                    ApplicationPartManager applicationPartManager = (ApplicationPartManager)descriptor.ImplementationInstance!;

                    applicationPartManager.ApplicationParts.Add(
                        new AssemblyPart(
                            typeof(BreadcrumbTestController).Assembly));
                }));
    }
}


internal sealed class BreadcrumbOptionsBuilder
{
    public static BreadcrumbOptionsBuilder Create() => new();
}
