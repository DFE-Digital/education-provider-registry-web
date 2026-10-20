using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Views.Shared.Components.Breadcrumbs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Views.Shared.Components.Breadcrumbs;

public sealed class BreadcrumbsViewComponentTests
{
    [Fact]
    public void Constructor_Throws_When_StoreIsNull()
    {
        // arrange  
        Func<BreadcrumbsViewComponent> construct = () =>
            new(store: null!);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Invoke_LoadsJourneyFromStore()
    {
        // arrange
        BreadcrumbJourney journey = new();

        Mock<IBreadcrumbJourneyStore> store = new();

        store.Setup(x => x.Load())
            .Returns(journey);

        BreadcrumbsViewComponent sut =
            new(store.Object);

        // act
        sut.Invoke();

        // assert
        store.Verify(
            x => x.Load(),
            Times.Once);
    }

    [Fact]
    public void Invoke_WhenJourneyContainsSingleItem_ReturnsEmptyBreadcrumbCollection()
    {
        // arrange
        BreadcrumbJourney journey = new()
        {
            Items =
            [
                new BreadcrumbItem(
                    "Details",
                    "/details")
            ]
        };

        Mock<IBreadcrumbJourneyStore> store = new();

        store.Setup(x => x.Load())
            .Returns(journey);

        BreadcrumbsViewComponent sut =
            new(store.Object);

        // act
        ViewViewComponentResult result =
            Assert.IsType<ViewViewComponentResult>(
                sut.Invoke());

        IReadOnlyCollection<BreadcrumbItem> model =
            Assert.IsType<IReadOnlyCollection<BreadcrumbItem>>(
                result.ViewData!.Model, exactMatch: false);

        // assert
        Assert.Empty(model);
    }

    [Fact]
    public void Invoke_WhenJourneyContainsMultipleItems_ReturnsAllExceptLastItem()
    {
        // arrange
        BreadcrumbItem search =
            new(
                "Search",
                "/search");

        BreadcrumbItem results =
            new(
                "Results",
                "/search/results");

        BreadcrumbItem details =
            new(
                "Details",
                "/details");

        BreadcrumbJourney journey = new()
        {
            Items =
            [
                search,
                results,
                details
            ]
        };

        Mock<IBreadcrumbJourneyStore> store = new();

        store.Setup(x => x.Load())
            .Returns(journey);

        BreadcrumbsViewComponent sut =
            new(store.Object);

        // act
        ViewViewComponentResult result =
            Assert.IsType<ViewViewComponentResult>(
                sut.Invoke());

        IReadOnlyCollection<BreadcrumbItem> model =
            Assert.IsAssignableFrom<IReadOnlyCollection<BreadcrumbItem>>(
                result.ViewData!.Model);

        // assert
        Assert.Equal(2, model.Count);

        Assert.Equal(
            [search, results],
            model);
    }

    [Fact]
    public void Invoke_ReturnsViewComponentResult()
    {
        // arrange
        Mock<IBreadcrumbJourneyStore> store = new();

        store.Setup(x => x.Load())
            .Returns(new BreadcrumbJourney());

        BreadcrumbsViewComponent sut =
            new(store.Object);

        // act
        IViewComponentResult result =
            sut.Invoke();

        // assert
        Assert.IsType<ViewViewComponentResult>(result);
    }
}
