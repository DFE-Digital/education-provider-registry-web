using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
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

        SiteAddressModel addressVo = new(
            Name: "Test School",
            AddressLine1: "Street",
            AddressLine2: "Street 2",
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
        Assert.Equal("/establishments/999999", table.CaptionLinkUrl);
    }

    [Fact]
    public void MapItem_CreatesRootRelativeEstablishmentLink()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult("123456", "Test School");

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        Assert.StartsWith("/", table.CaptionLinkUrl);
        Assert.Equal(
            "/establishments/123456",
            table.CaptionLinkUrl);
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
        TableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "URN");
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
        TableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Type");
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
        TableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Address");
        Assert.Equal("Street, Street 2, Town, County, AB1 2CD", row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_AddsLocalAuthorityRow_WithLink()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        TableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Local authority");

        // assert
        Assert.Equal("LA Name", row.Cells[1].Text);
        Assert.Equal("/la/123", row.Cells[1].Href);
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
        TableRow row = table.Rows.Single(tableRow => tableRow.Cells[0].Text == "Part of a group");

        Assert.Equal("Group Name", row.Cells[1].Text);
        Assert.Equal("/groups/G123", row.Cells[1].Href);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenCollectionContainsNullResult()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        IReadOnlyCollection<EstablishmentSearchResult> input =
        [
            null!
        ];

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(input));
    }

    [Fact]
    public void MapItem_ConfiguresColumnsCorrectly()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        Assert.Collection(
            table.Columns,
            column =>
            {
                Assert.Equal("Name", column.Text);
                Assert.True(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Value", column.Text);
                Assert.False(column.IsRowHeader);
            });
    }

    [Fact]
    public void MapItem_AddsExpectedRows()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();
        EstablishmentSearchResult input = MakeResult();

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        Assert.Collection(
            table.Rows,
            row => Assert.Equal("URN", row.Cells[0].Text),
            row => Assert.Equal("Type", row.Cells[0].Text),
            row => Assert.Equal("Address", row.Cells[0].Text),
            row => Assert.Equal("Local authority", row.Cells[0].Text),
            row => Assert.Equal("Part of a group", row.Cells[0].Text));
    }

    [Fact]
    public void MapItem_ExcludesBlankAddressParts()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        SiteAddressModel address = new(
            Name: "Test School",
            AddressLine1: "Street",
            AddressLine2: "Street 2",
            Town: "Town",
            County: " ",
            Postcode: "AB1 2CD");

        EstablishmentSearchResult input = new(
            new UniqueReferenceNumber("111111"),
            new Name("Test School"),
            address,
            new EstablishmentType("Academy"),
            new GroupDetail("Group Name", "G123"),
            new LocalAuthority("LA Name", "123"));

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        TableRow row = table.Rows.Single(
            tableRow => tableRow.Cells[0].Text == "Address");

        Assert.Equal("Street, Street 2, Town, AB1 2CD", row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_UsesEmptyAddress_WhenAddressIsNull()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        EstablishmentSearchResult input = new(
            new UniqueReferenceNumber("111111"),
            new Name("Test School"),
            null,
            new EstablishmentType("Academy"),
            new GroupDetail("Group Name", "G123"),
            new LocalAuthority("LA Name", "123"));

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        TableRow row = table.Rows.Single(
            tableRow => tableRow.Cells[0].Text == "Address");

        Assert.Equal(string.Empty, row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_UsesNullTypeText_WhenTypeIsNull()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        EstablishmentSearchResult input = new(
            new UniqueReferenceNumber("111111"),
            new Name("Test School"),
            new SiteAddressModel("Test School", "Street", "Street 2", "Town", "County", "AB1 2CD"),
            null,
            new GroupDetail("Group Name", "G123"),
            new LocalAuthority("LA Name", "123"));

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        TableRow row = table.Rows.Single(
            tableRow => tableRow.Cells[0].Text == "Type");

        Assert.Null(row.Cells[1].Text);
    }

    [Fact]
    public void MapItem_UsesNullLocalAuthorityValues_WhenLocalAuthorityIsNull()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        EstablishmentSearchResult input = new(
            new UniqueReferenceNumber("111111"),
            new Name("Test School"),
            new SiteAddressModel("Test School", "Street", "Street 2", "Town", "County", "AB1 2CD"),
            new EstablishmentType("Academy"),
            new GroupDetail("Group Name", "G123"),
            null);

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        TableRow row = table.Rows.Single(
            tableRow => tableRow.Cells[0].Text == "Local authority");

        Assert.Null(row.Cells[1].Text);
        Assert.Null(row.Cells[1].Href);
    }

    [Fact]
    public void MapItem_UsesNullGroupValues_WhenGroupIsNull()
    {
        // arrange
        EstablishmentSearchResultsToViewModelMapper mapper = new();

        EstablishmentSearchResult input = new(
            new UniqueReferenceNumber("111111"),
            new Name("Test School"),
            new SiteAddressModel("Test School", "Street", "Street 2", "Town", "County", "AB1 2CD"),
            new EstablishmentType("Academy"),
            null,
            new LocalAuthority("LA Name", "123"));

        // act
        GovUkTable table = mapper.Map([input])[0];

        // assert
        TableRow row = table.Rows.Single(
            tableRow => tableRow.Cells[0].Text == "Part of a group");

        Assert.Null(row.Cells[1].Text);
        Assert.Null(row.Cells[1].Href);
    }
}
