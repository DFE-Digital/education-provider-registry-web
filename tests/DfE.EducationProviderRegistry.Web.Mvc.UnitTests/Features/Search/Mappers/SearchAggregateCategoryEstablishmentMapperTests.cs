using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchAggregateCategoryEstablishmentMapperTests
{
    private readonly SearchAggregateCategoryEstablishmentMapper _mapper = new();

    private static SearchAggregateResult MakeEstablishment()
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier("111111"),
            new Name("School"),
            new SearchAddress("Address"),
            new SearchType("Academy", 1),
            null,
            null,
            new SearchCategory("Establishment"),
            0);
    }

    private static SearchAggregateResult MakeGroup()
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier("222222"),
            new Name("Group"),
            new SearchAddress("Address"),
            new SearchType("MAT", 1),
            null,
            null,
            new SearchCategory("Group"),
            5);
    }

    private static SearchAggregateResult MakeResult()
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier("111111"),
            new Name("Test School"),
            new SearchAddress("Street, Town, AB1 2CD"),
            new SearchType("Academy", 1),
            new GroupDetail("Group Name", "222222"),
            new SearchLocalAuthority("LA Name"),
            new SearchCategory("Establishment"),
            0);
    }

    [Fact]
    public void CanMap_ReturnsTrue_ForEstablishment()
    {
        Assert.True(_mapper.CanMap(MakeEstablishment()));
    }

    [Fact]
    public void CanMap_ReturnsFalse_ForGroup()
    {
        Assert.False(_mapper.CanMap(MakeGroup()));
    }

    [Fact]
    public void Map_SetsCaptionAndLink()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        Assert.Equal("Test School", table.Caption);
        Assert.Equal("/establishments/111111", table.CaptionLinkUrl);
    }

    [Fact]
    public void Map_ConfiguresColumns()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        Assert.Collection(
            table.Columns,
            c =>
            {
                Assert.Equal("Name", c.Text);
                Assert.True(c.IsRowHeader);
            },
            c =>
            {
                Assert.Equal("Value", c.Text);
                Assert.False(c.IsRowHeader);
            });
    }

    [Fact]
    public void Map_AddsUrnRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "URN");

        Assert.Equal("111111", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsTypeRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Type");

        Assert.Equal("Academy", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsAddressRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Address");

        Assert.Equal("Street, Town, AB1 2CD", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsLocalAuthorityRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Local authority");

        Assert.Equal("LA Name", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsGroupRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Part of a group");

        Assert.Equal("Group Name", row.Cells[1].Text);
        Assert.Equal("/groups/222222", row.Cells[1].Href);
    }

    [Fact]
    public void Map_UsesNullType_WhenTypeIsNull()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("111111"),
                new Name("Test"),
                new SearchAddress("Address"),
                null,
                null,
                null,
                new SearchCategory("Establishment"),
                0);

        GovUkTable table = _mapper.Map(input);

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Type");

        Assert.Null(row.Cells[1].Text);
    }

    [Fact]
    public void Map_UsesNullGroupValues_WhenGroupIsNull()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("111111"),
                new Name("Test"),
                new SearchAddress("Address"),
                new SearchType("Academy", 1),
                null,
                null,
                new SearchCategory("Establishment"),
                0);

        GovUkTable table = _mapper.Map(input);

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Part of a group");

        Assert.Null(row.Cells[1].Text);
        Assert.Null(row.Cells[1].Href);
    }

    [Fact]
    public void Map_UsesNullLocalAuthority_WhenLocalAuthorityIsNull()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("111111"),
                new Name("Test"),
                new SearchAddress("Address"),
                new SearchType("Academy", 1),
                null,
                null,
                new SearchCategory("Establishment"),
                0);

        GovUkTable table = _mapper.Map(input);

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Local authority");

        Assert.Null(row.Cells[1].Text);
    }
}
