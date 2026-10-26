using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public sealed class BreadcrumbOptionsTests
{
    [Fact]
    public void Configuration_Binds_To_BreadcrumbOptions()
    {
        // Arrange
        IReadOnlyDictionary<string, string?> configurationValues = BreadcrumbOptionsTestDoubles.Stub();

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

        ServiceCollection services = new();

        services.AddBreadcrumbDependencies(configuration);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Act
        BreadcrumbOptions options =
            serviceProvider
                .GetRequiredService<IOptions<BreadcrumbOptions>>()
                .Value;

        // Assert
        Assert.Equal(3, options.Rules.Count);

        BreadcrumbRouteRule searchResults = options.Rules[0];

        Assert.Equal("/search/results", searchResults.PathPattern);
        Assert.Equal("Search Results", searchResults.Title);
        Assert.Null(searchResults.TitleModelProperty);
        Assert.Equal(
            BreadcrumbJourneyResetMode.DirectNavigationOnly,
            searchResults.JourneyResetMode);

        Assert.Equal(2, searchResults.BaseJourney.Count);

        Assert.Equal("Home", searchResults.BaseJourney[0].Title);
        Assert.Equal("/", searchResults.BaseJourney[0].Url);

        Assert.Equal("Search", searchResults.BaseJourney[1].Title);
        Assert.Equal("/search", searchResults.BaseJourney[1].Url);

        BreadcrumbRouteRule establishments = options.Rules[1];

        Assert.Equal("/establishments", establishments.PathPattern);
        Assert.Null(establishments.Title);
        Assert.Equal("Heading", establishments.TitleModelProperty);
        Assert.Equal(
            BreadcrumbJourneyResetMode.Always,
            establishments.JourneyResetMode);

        Assert.Empty(establishments.BaseJourney);

        BreadcrumbRouteRule groups = options.Rules[2];

        Assert.Equal("/groups", groups.PathPattern);
        Assert.Equal("Heading", groups.TitleModelProperty);
        Assert.Equal(
            BreadcrumbJourneyResetMode.None,
            groups.JourneyResetMode);
        Assert.Empty(groups.BaseJourney);
    }
}
