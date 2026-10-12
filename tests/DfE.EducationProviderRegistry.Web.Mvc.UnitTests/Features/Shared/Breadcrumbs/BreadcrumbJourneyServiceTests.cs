using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Services;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Breadcrumbs;

public sealed class SessionBreadcrumbJourneyServiceTests
{
    private readonly SessionBreadcrumbJourneyService _sut;

    public SessionBreadcrumbJourneyServiceTests()
    {
        _sut = new SessionBreadcrumbJourneyService(
            new SessionTestDouble());
    }

    [Fact]
    public void Constructor_WhenSessionIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SessionBreadcrumbJourneyService(null!));
    }

    [Fact]
    public void Get_WhenNoJourneyExists_ReturnsEmptyJourney()
    {
        BreadcrumbJourney result = _sut.Get();

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Clear_WhenJourneyExists_RemovesJourney()
    {
        _sut.StartSearchJourney(
            "/search/results?what=test");

        _sut.Clear();

        BreadcrumbJourney result = _sut.Get();

        Assert.Empty(result.Items);
    }

    [Fact]
    public void StartSearchJourney_WhenUrlIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => _sut.StartSearchJourney(null!));
    }

    [Fact]
    public void StartSearchJourney_WhenUrlIsEmpty_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => _sut.StartSearchJourney(string.Empty));
    }

    [Fact]
    public void StartSearchJourney_CreatesExpectedJourney()
    {
        _sut.StartSearchJourney(
            "/search/results?what=test");

        BreadcrumbJourney result = _sut.Get();

        Assert.Collection(
            result.Items,

            item =>
            {
                Assert.Equal("Home", item.Title);
                Assert.Equal("/", item.Url);
            },

            item =>
            {
                Assert.Equal("Search", item.Title);
                Assert.Equal("/search", item.Url);
            },

            item =>
            {
                Assert.Equal("Search Results", item.Title);
                Assert.Equal(
                    "/search/results?what=test",
                    item.Url);
            });
    }

    [Fact]
    public void Visit_WhenPageIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => _sut.Visit(null!));
    }

    [Fact]
    public void Visit_WhenJourneyIsEmpty_StartsJourney()
    {
        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/123"));

        BreadcrumbJourney result = _sut.Get();

        Assert.Collection(
            result.Items,
            item =>
            {
                Assert.Equal(
                    "Academy A",
                    item.Title);

                Assert.Equal(
                    "/establishments/123",
                    item.Url);
            });
    }

    [Fact]
    public void Visit_WhenPageDoesNotExist_AppendsPage()
    {
        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/1"));

        _sut.Visit(
            new BreadcrumbItem(
                "Group A",
                "/groups/1"));

        BreadcrumbJourney result = _sut.Get();

        Assert.Collection(
            result.Items,
            item =>
                Assert.Equal(
                    "/establishments/1",
                    item.Url),

            item =>
                Assert.Equal(
                    "/groups/1",
                    item.Url));
    }

    [Fact]
    public void Visit_WhenRefreshingCurrentPage_DoesNothing()
    {
        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/1"));

        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/1"));

        BreadcrumbJourney result = _sut.Get();

        Assert.Single(result.Items);
    }

    [Fact]
    public void Visit_WhenRefreshingCurrentPageWithDifferentCase_DoesNothing()
    {
        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/ESTABLISHMENTS/1"));

        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/1"));

        BreadcrumbJourney result = _sut.Get();

        Assert.Single(result.Items);
    }

    [Fact]
    public void Visit_WhenPageAlreadyExists_TruncatesJourney()
    {
        _sut.Visit(
            new BreadcrumbItem(
                "Academy A",
                "/establishments/1"));

        _sut.Visit(
            new BreadcrumbItem(
                "Group A",
                "/groups/1"));

        _sut.Visit(
            new BreadcrumbItem(
                "Academy B",
                "/establishments/2"));

        _sut.Visit(
            new BreadcrumbItem(
                "Group A",
                "/groups/1"));

        BreadcrumbJourney result = _sut.Get();

        Assert.Collection(
            result.Items,

            item =>
                Assert.Equal(
                    "/establishments/1",
                    item.Url),

            item =>
                Assert.Equal(
                    "/groups/1",
                    item.Url));
    }
}
