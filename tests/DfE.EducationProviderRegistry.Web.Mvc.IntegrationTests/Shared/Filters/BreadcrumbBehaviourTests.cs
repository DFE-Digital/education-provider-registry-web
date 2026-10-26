using System.Net;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbBehaviourTests
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

    [Fact]
    public async Task Get_WhenIsViewResult_And_RouteDoesNotMatchConfiguration_DoesNotAccessBreadcrumbJourney()
    {
        // Arrange
        Mock<IBreadcrumbJourneyStore> store =
            IBreadCrumbJourneyStoreTestDoubles.Default();

        BreadcrumbOptions options =
            BreadcrumbOptionsBuilder.Create()
                .WithRule("/some-other-route")
                .Build();

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                    builder
                        .UseBreadcrumbTestConfiguration(options)
                        .UseBreadcrumbTestMvcInfrastructure()
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

    public static IWebHostBuilder UseBreadcrumbTestConfiguration(
        this IWebHostBuilder builder,
        BreadcrumbOptions options)
    {
        // Replace the specific Options
        // - Program registers multiple CompositionRoots, by replacing IConfiguration we lose valid defaults
        // - Rulebinding of BreadcrumbOptions tested separately
        return builder.ConfigureTestServices((services) =>
        {
            services.RemoveAll<IConfigureOptions<BreadcrumbOptions>>();

            services.Configure<BreadcrumbOptions>((configuredOptions) =>
            {
                configuredOptions.Rules = options.Rules;
            });
        });
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
    List<BreadcrumbRouteRule> _rules;
    public BreadcrumbOptionsBuilder()
    {
        _rules = [];
    }

    public static BreadcrumbOptionsBuilder Create() => new();
    public BreadcrumbOptionsBuilder WithRule(string path, BreadcrumbJourneyResetMode? resetMode = null)
    {
        BreadcrumbRouteRule rule = new()
        {
            PathPattern = path
        };

        if (resetMode is not null)
        {
            rule.JourneyResetMode = resetMode.Value;
        }

        _rules.Add(rule);

        return this;
    }

    public BreadcrumbOptions Build()
    {
        return new()
        {
            Rules = _rules
        };
    }
}
