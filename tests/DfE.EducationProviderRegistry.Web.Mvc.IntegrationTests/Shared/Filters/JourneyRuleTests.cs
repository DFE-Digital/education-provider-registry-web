using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.Infrastructure.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters.TestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class JourneyRuleTests
{
    [Fact]
    public async Task Specific_Rule_Configured()
    {
        BreadcrumbRouteRule rule =
            BreadcrumbRouteRuleBuilder.Create(pathPattern: "/")
                .WithPrefix("Home1", "/home1")
                .WithPrefix("Home2", "/home2")
                .Build();

        BreadcrumbOptions options =
            BreadcrumbOptionsTestDoubles.Create([rule]);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.Default
                .WithWebHostBuilder((builder) =>
                {
                    builder
                        .UseBreadcrumbConfiguration(options);
                });

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponse = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document = await httpResponse.AssertSuccessfulHtmlResponseAsync();
        Assert.NotNull(document.QuerySelectorAll(".govuk-breadcrumbs"));
    }
}

public sealed class BreadcrumbsComponent
{
    private readonly IHtmlDocument _document;

    public BreadcrumbsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

}
