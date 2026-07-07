using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Xunit;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class EstablishmentSearchResultsToViewModelMapperTests
{
    private static EstablishmentSearchResult MakeResult(
        string urn = "111111",
        string name = "Test School")
    {
        UniqueReferenceNumber urnVo = new(urn);
        Name nameVo = new(name);

        Address addressVo = new(
            Street: "Street",
            Town: "Town",
            County: "County",
            Postcode: "AB1 2CD");

        EstablishmentType typeVo = new("Academy");

        GroupDetail groupVo = new(
            partOfName: "Group Name",
            partOfCode: "G123");

        LocalAuthority laVo = new(
            localAuthorityName: "LA Name",
            localAuthorityCode: "123");

        return new EstablishmentSearchResult(
            urnVo,
            nameVo,
            addressVo,
            typeVo,
            groupVo,
            laVo);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_ReturnsEmptyList_WhenInputIsEmpty()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        // act
        List<GovUkTable> result = mapper.Map([]);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MapsEachResult_ToAGovUkTable()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        IReadOnlyCollection<EstablishmentSearchResult> input =
        [
            MakeResult("111111", "School A"),
            MakeResult("222222", "School B")
        ];

        // act
        List<GovUkTable> tables = mapper.Map(input);

        // assert
        Assert.Equal(2, tables.Count);
        Assert.Equal("School A", tables[0].Caption);
        Assert.Equal("School B", tables[1].Caption);
    }

    [Fact]
    public void MapItem_SetsCaptionAndLinkCorrectly()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult("999999", "My School");

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        Assert.Equal("My School", table.Caption);
        Assert.Equal("establishment/999999", table.CaptionLinkUrl);
    }

    [Fact]
    public void MapItem_AddsUrnRow()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult("555555", "Test");

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        GovUkTableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "URN");
        Assert.Equal("555555", row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_AddsTypeRow()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        GovUkTableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Type");
        Assert.Equal("Academy", row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_AddsAddressRow()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        GovUkTableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Address");
        Assert.Equal("Street County AB1 2CD", row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_AddsLocalAuthorityRow_WithLink()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        GovUkTableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Local authority");

        // assert
        Assert.Equal("LA Name", row.Cells[1].Text);
        Assert.Equal("/la/123", row.Cells[1].LinkUrl);
    }

    [Fact]
    public void MapItem_AddsGroupRow_WithLink()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        GovUkTableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Part of");

        Assert.Equal("Group Name", row.Cells[1].Text);
        Assert.Equal("/group/G123", row.Cells[1].LinkUrl);
    }
}
