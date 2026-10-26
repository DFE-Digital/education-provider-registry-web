using System.Net;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Org.BouncyCastle.Utilities.Collections;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbBehaviourIntegrationTests
{
    [Fact]
    public async Task Get_WhenResultIsNotViewResult_DoesNotAccessBreadcrumbJourney()
    {
        Mock<IBreadcrumbJourneyStore> store = IBreadCrumbJourneyStoreTestDoubles.Default();

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbTestMvcInfrastructure()
                        .ConfigureTestServices((services) =>
                        {
                            services.RemoveAll<IBreadcrumbJourneyStore>();
                            services.AddSingleton(_ => store.Object);
                        }));

        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/breadcrumb-tests/redirect", TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        Assert.Empty(store.Invocations);
    }
}

internal static class IBreadCrumbJourneyStoreTestDoubles
{
    public static Mock<IBreadcrumbJourneyStore> Default() => new();
}

internal static class BreadcrumbTestDependencies
{
    public static IWebHostBuilder UseBreadcrumbTestMvcInfrastructure(this IWebHostBuilder builder)
    {
        return
            builder
                // To avoid dependancy on real controllers with real dependencies- isolate to MVC pipeline
                .AddBreadcrumbsTestController()
                // To avoid MVC View Handling as not part of breadcrumb behaviour - Replace handler that acts on IAsyncResultFilter when ViewResult
                .AddBreadcrumbViewResultNoOp();


    }

    private static IWebHostBuilder AddBreadcrumbViewResultNoOp(this IWebHostBuilder builder)
    {
        return builder.ConfigureTestServices((services) =>
        {
            services.RemoveAll<IActionResultExecutor<ViewResult>>();

            services.AddSingleton<
                IActionResultExecutor<ViewResult>,
                NoOpViewResultExecutor>();
        });
    }
    private static IWebHostBuilder AddBreadcrumbsTestController(this IWebHostBuilder program)
    {
        return program.ConfigureServices((services) =>
        {
            ServiceDescriptor descriptor = services.Single(
                (descriptor) =>
                    descriptor.ServiceType == typeof(ApplicationPartManager));

            ApplicationPartManager applicationPartManager = (ApplicationPartManager)descriptor.ImplementationInstance!;

            applicationPartManager.ApplicationParts.Add(
                new AssemblyPart(
                    typeof(BreadcrumbTestController).Assembly));
        });
    }
}


internal sealed class BreadcrumbOptionsBuilder
{
    public static BreadcrumbOptionsBuilder Create() => new();
}
