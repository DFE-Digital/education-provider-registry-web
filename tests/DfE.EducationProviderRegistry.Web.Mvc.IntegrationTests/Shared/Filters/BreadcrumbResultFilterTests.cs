using System.Net;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.Infrastructure.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbResultFilterTests
{
    [Fact]
    public async Task Get_WhenResultIsNotViewResult_DoesNotAccessBreadcrumbJourney()
    {
        Mock<IBreadcrumbJourneyStore> store = IBreadCrumbJourneyStoreTestDoubles.Default();

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbStubMvcInfrastructure()
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

    [Fact]
    public async Task Get_WhenIsViewResult_And_RouteDoesNotMatchConfiguration_DoesNotAccessBreadcrumbJourney()
    {
        // Arrange
        Mock<IBreadcrumbJourneyStore> store =
            IBreadCrumbJourneyStoreTestDoubles.Default();

        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder
                .Create("/some-other-route")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbStubMvcInfrastructure()
                        .ConfigureTestServices((services) =>
                        {
                            services.RemoveAll<IBreadcrumbJourneyStore>();
                            services.AddSingleton(_ => store.Object);
                        }));

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync(
            "/breadcrumb-tests/view",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(store.Invocations);
    }

    [Fact]
    public async Task Get_WhenIsViewResult_And_Route_Resolves_Stores_BreadcrumbJourney()
    {
        // Arrange
        const string matchesRoute = "/breadcrumb-tests/view";

        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder
                .Create(matchesRoute)
                .WithTitle("Breadcrumb test")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                {
                    builder
                        .UseBreadcrumbConfiguration(options)
                        .UseBreadcrumbStubMvcInfrastructure();
                });


        HttpClient client = factory.CreateClient();

        // Act
        HttpResponseMessage response =
            await client.GetAsync(
                matchesRoute,
                TestContext.Current.CancellationToken);

        // Assert
        // Session endpoint exists solely to retrieve ISession details
        HttpResponseMessage sessionResponse =
            await client.GetAsync("/breadcrumb-tests/session", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
        sessionResponse.EnsureSuccessStatusCode();

        byte[] journey =
            await sessionResponse.Content.ReadAsByteArrayAsync(
                TestContext.Current.CancellationToken);

        Assert.NotEmpty(journey);
    }
}
