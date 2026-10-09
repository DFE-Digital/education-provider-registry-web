using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Stores;
using Moq;
using Xunit;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Breadcrumbs;

public sealed class BreadcrumbJourneyServiceTests
{
    private readonly Mock<IBreadcrumbJourneyStore> _store = new();
    private readonly BreadcrumbJourneyService _sut;

    public BreadcrumbJourneyServiceTests()
    {
        _sut = new BreadcrumbJourneyService(_store.Object);
    }

    [Fact]
    public void Get_Returns_Journey()
    {
        BreadcrumbJourney expected = new();

        _store
            .Setup(x => x.Get())
            .Returns(expected);

        BreadcrumbJourney result = _sut.Get();

        Assert.Equal(expected, result);
        _store.Verify(
            x => x.Get(),
            Times.Once);
    }

    [Fact]
    public void Clear_Calls_StoreClear()
    {
        _sut.Clear();

        _store.Verify(
            x => x.Clear(),
            Times.Once);
    }

    [Fact]
    public void StartSearchJourney_CreatesExpectedJourney()
    {
        _sut.StartSearchJourney(
            "/search/results?what=test");

        _store.Verify(
            x => x.Save(
                It.Is<BreadcrumbJourney>(j =>
                    j.Items.Count == 3 &&
                    j.Items[0].Title == "Home" &&
                    j.Items[1].Title == "Search" &&
                    j.Items[2].Title == "Search Results" &&
                    j.Items[2].Url ==
                    "/search/results?what=test")),
            Times.Once);
    }

    [Fact]
    public void StartSearchJourney_WhenUrlIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.StartSearchJourney(null!));
    }

    [Fact]
    public void StartSearchJourney_WhenUrlIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.StartSearchJourney(""));
    }

    [Fact]
    public void NavigateTo_WhenJourneyEmpty_AddsDestination()
    {
        BreadcrumbJourney journey = new();

        _store
            .Setup(x => x.Get())
            .Returns(journey);

        _sut.NavigateTo(
            new BreadcrumbItem(
                "Academy",
                "/establishments/123"));

        Assert.Single(journey.Items);

        Assert.Equal(
            "Academy",
            journey.Items[0].Title);

        _store.Verify(
            x => x.Save(journey),
            Times.Once);
    }

    [Fact]
    public void NavigateTo_WhenRefreshing_DoesNothing()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new(
                        "Academy",
                        "/establishments/123")
                ]
            };

        _store
            .Setup(x => x.Get())
            .Returns(journey);

        _sut.NavigateTo(
            new BreadcrumbItem(
                "Academy",
                "/establishments/123"));

        Assert.Single(journey.Items);

        _store.Verify(
            x => x.Save(It.IsAny<BreadcrumbJourney>()),
            Times.Never);
    }

    [Fact]
    public void NavigateTo_WhenItemAlreadyExists_TruncatesJourney()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new("Home", "/"),
                    new("Search", "/search"),
                    new("Group A", "/groups/1"),
                    new("Academy A", "/establishments/1")
                ]
            };

        _store
            .Setup(x => x.Get())
            .Returns(journey);

        _sut.NavigateTo(
            new BreadcrumbItem(
                "Group A",
                "/groups/1"));

        Assert.Equal(
            3,
            journey.Items.Count);

        Assert.Equal(
            "/groups/1",
            journey.Items.Last().Url);

        _store.Verify(
            x => x.Save(journey),
            Times.Once);
    }

    [Fact]
    public void NavigateTo_WhenItemNotInJourney_AppendsItem()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new("Home", "/")
                ]
            };

        _store
            .Setup(x => x.Get())
            .Returns(journey);

        _sut.NavigateTo(
            new BreadcrumbItem(
                "Academy",
                "/establishments/1"));

        Assert.Equal(
            2,
            journey.Items.Count);

        Assert.Equal(
            "/establishments/1",
            journey.Items.Last().Url);

        _store.Verify(
            x => x.Save(journey),
            Times.Once);
    }
}
