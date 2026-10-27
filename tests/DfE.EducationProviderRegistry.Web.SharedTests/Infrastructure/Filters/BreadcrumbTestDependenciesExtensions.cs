using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Filters;

public static class BreadcrumbTestDependenciesExtensions
{
    public static IWebHostBuilder UseBreadcrumbStubMvcInfrastructure(this IWebHostBuilder builder)
    {
        return
            builder
                // To avoid dependancy on real controllers with real dependencies- isolate to MVC pipeline
                .AddBreadcrumbsTestController()
                // To avoid MVC View Handling as not part of breadcrumb behaviour - Replace handler that acts on IAsyncResultFilter when ViewResult
                .AddBreadcrumbViewResultNoOp();
    }

    public static IWebHostBuilder UseBreadcrumbConfiguration(
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

    public static IWebHostBuilder UseBreadcrumbExistingJourney(this IWebHostBuilder builder, IBreadcrumbJourneyStore store)
    {
        return builder.ConfigureTestServices((services) =>
        {
            services.RemoveAll<IBreadcrumbJourneyStore>();

            services.AddScoped<IBreadcrumbJourneyStore>(_ => store);
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
