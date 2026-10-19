using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;
using Microsoft.AspNetCore.Http;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.Store;

public sealed class SessionBreadcrumbJourneyStoreTests
{
    [Fact]
    public void Constructor_WhenSessionIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SessionBreadcrumbJourneyStore(null!));
    }

    [Fact]
    public void Load_WhenSessionIsEmpty_ShouldReturnEmptyJourney()
    {
        // Arrange
        SessionTestDouble session =
            SessionTestDouble.Create();

        SessionBreadcrumbJourneyStore sut =
            new(session);

        // Act
        BreadcrumbJourney result =
            sut.Load();

        // Assert
        Assert.Empty(result.Items);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Load_WhenStoredJsonIsWhitespace_ShouldReturnEmptyJourney(
    string json)
    {
        // Arrange
        SessionTestDouble session = SessionTestDouble.Create();

        session.SetString("BreadcrumbJourney", json);

        SessionBreadcrumbJourneyStore sut =
            new(session);

        // Act
        BreadcrumbJourney result = sut.Load();

        // Assert
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Save_ShouldPersistJourney()
    {
        // Arrange
        SessionTestDouble session =
            SessionTestDouble.Create();

        SessionBreadcrumbJourneyStore sut =
            new(session);

        BreadcrumbJourney journey =
            new();

        journey.Items.Add(
            new BreadcrumbItem(
                "Search Results",
                "/search/results"));

        // Act
        sut.Save(journey);

        BreadcrumbJourney result =
            sut.Load();

        // Assert
        Assert.Single(result.Items);

        Assert.Equal(
            "Search Results",
            result.Items[0].Title);

        Assert.Equal(
            "/search/results",
            result.Items[0].Url);
    }

    [Fact]
    public void Clear_ShouldRemoveStoredJourney()
    {
        // Arrange
        SessionTestDouble session =
            SessionTestDouble.Create();

        SessionBreadcrumbJourneyStore sut =
            new(session);

        BreadcrumbJourney journey =
            new();

        journey.Items.Add(
            new BreadcrumbItem(
                "Search Results",
                "/search/results"));

        sut.Save(journey);

        // Act
        sut.Clear();

        BreadcrumbJourney result =
            sut.Load();

        // Assert
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Load_AfterSave_ShouldReturnSameJourney()
    {
        // Arrange
        SessionTestDouble session =
            SessionTestDouble.Create();

        SessionBreadcrumbJourneyStore sut =
            new(session);

        BreadcrumbJourney expected =
            new();

        expected.Items.Add(
            new BreadcrumbItem(
                "Search",
                "/search"));

        expected.Items.Add(
            new BreadcrumbItem(
                "Search Results",
                "/search/results"));

        // Act
        sut.Save(expected);

        BreadcrumbJourney actual =
            sut.Load();

        // Assert
        Assert.Equal(
            expected.Items.Count,
            actual.Items.Count);

        Assert.Equal(
            expected.Items[0],
            actual.Items[0]);

        Assert.Equal(
            expected.Items[1],
            actual.Items[1]);
    }
}
