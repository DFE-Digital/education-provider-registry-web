using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Filters;
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
        BreadcrumbOptions breadCrumbOptions = BreadcrumbOptionsTestDoubles.Create([
            BreadcrumbRouteRuleBuilder
                .Create("/search/results")
                .WithTitle("Search Results")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.DirectNavigationOnly)
                .WithAddToBaseJourney("Home", "/")
                .WithAddToBaseJourney("Search", "/search")
                .Build(),

            BreadcrumbRouteRuleBuilder
                .Create("/establishments")
                .WithTitleModelProperty("Heading")
                .WithJourneyResetMode(BreadcrumbJourneyResetMode.Always)
                .Build(),

            BreadcrumbRouteRuleBuilder
                .Create("/groups")
                .WithTitleModelProperty("Heading")
                .Build()
        ]);

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(BreadcrumbOptionsConfigurationAdapter.ToConfiguration(breadCrumbOptions))
                .Build();

        ServiceCollection services = new();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

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
