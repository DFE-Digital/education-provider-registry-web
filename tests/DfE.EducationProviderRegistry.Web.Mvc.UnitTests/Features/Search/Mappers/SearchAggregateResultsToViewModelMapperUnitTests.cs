using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchAggregateResultsToViewModelMapperTests
{
    private static SearchAggregateResult MakeEstablishmentResult(
        string identifier = "111111",
        string name = "Test School")
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier(identifier),
            new Name(name),
            new SearchAddress("Street, Street 2, Town, County, AB1 2CD"),
            new SearchType("Academy", 1),
            new GroupDetail("Group Name", "1234"),
            new SearchLocalAuthority("LA Name"),
            new SearchCategory("Establishment"),
            0);
    }

    private static SearchAggregateResult MakeGroupResult(
        string identifier = "1234",
        string name = "Test Group")
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier(identifier),
            new Name(name),
            new SearchAddress("Group Address"),
            new SearchType("MAT", 1),
            null,
            null,
            new SearchCategory("Group"),
            25);
    }

    [Fact]
    public void Map_ReturnsEmptyList_WhenInputIsEmpty()
    {
        // arrange
        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper(),
                new SearchAggregateCategoryGroupMapper()
            ]);

        // act
        List<GovUkTable> result = mapper.Map([]);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MapsEachResult_ToAGovUkTable()
    {
        // arrange
        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper(),
                new SearchAggregateCategoryGroupMapper()
            ]);

        IReadOnlyCollection<SearchAggregateResult> input =
        [
            MakeEstablishmentResult("111111", "School A"),
            MakeEstablishmentResult("222222", "School B")
        ];

        // act
        List<GovUkTable> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);
        Assert.Equal("School A", result[0].Caption);
        Assert.Equal("School B", result[1].Caption);
    }

    [Fact]
    public void Map_UsesEstablishmentMapper_WhenCategoryIsEstablishment()
    {
        // arrange
        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper(),
                new SearchAggregateCategoryGroupMapper()
            ]);

        SearchAggregateResult input =
            MakeEstablishmentResult("999999", "My School");

        // act
        GovUkTable result = mapper.Map([input]).Single();

        // assert
        Assert.Equal("My School", result.Caption);
        Assert.Equal("/establishments/999999", result.CaptionLinkUrl);

        Assert.Contains(
            result.Rows,
            row => row.Cells[0].Text == "URN");
        Assert.DoesNotContain(
            result.Rows,
            row => row.Cells[0].Text == "Group ID");
    }

    [Fact]
    public void Map_UsesGroupMapper_WhenCategoryIsGroup()
    {
        // arrange
        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper(),
                new SearchAggregateCategoryGroupMapper()
            ]);

        SearchAggregateResult input =
            MakeGroupResult("1234", "My Group");

        // act
        GovUkTable result = mapper.Map([input]).Single();

        // assert
        Assert.Equal("My Group", result.Caption);
        Assert.Equal("/groups/1234", result.CaptionLinkUrl);

        Assert.Contains(
            result.Rows,
            row => row.Cells[0].Text == "Group ID");
        Assert.DoesNotContain(
           result.Rows,
           row => row.Cells[0].Text == "URN");
        Assert.Contains(
            result.Rows,
            row => row.Cells[0].Text == "Academies");
    }

    [Fact]
    public void Map_UsesCorrectMapper_ForEachItem_WhenInputContainsMixedCategories()
    {
        // arrange
        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper(),
                new SearchAggregateCategoryGroupMapper()
            ]);

        IReadOnlyCollection<SearchAggregateResult> input =
        [
            MakeEstablishmentResult("111111", "School A"),
            MakeGroupResult("1234", "Group A")
        ];

        // act
        List<GovUkTable> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Equal("/establishments/111111", result[0].CaptionLinkUrl);
        Assert.Equal("/groups/1234", result[1].CaptionLinkUrl);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenNoMapperRegisteredForCategory()
    {
        // arrange
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("1234"),
                new Name("Unknown"),
                null,
                null,
                null,
                null,
                new SearchCategory("Unknown"),
                0);

        SearchAggregateResultsToViewModelMapper mapper =
            new([]);

        // act / assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => mapper.Map([input]));

        Assert.Contains(
            "No mapper registered for Unknown",
            exception.Message);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenCategoryHasNoMatchingMapper()
    {
        // arrange
        SearchAggregateResult input =
            MakeGroupResult();

        SearchAggregateResultsToViewModelMapper mapper =
            new(
            [
                new SearchAggregateCategoryEstablishmentMapper()
            ]);

        // act / assert
        Assert.Throws<InvalidOperationException>(
            () => mapper.Map([input]));
    }
}