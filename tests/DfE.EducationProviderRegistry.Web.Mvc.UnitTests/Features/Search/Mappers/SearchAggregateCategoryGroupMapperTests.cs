using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchAggregateCategoryGroupMapperTests
{
    private readonly SearchAggregateCategoryGroupMapper _mapper = new();

    private static SearchAggregateResult MakeResult()
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier("222222"),
            new Name("Trust Name"),
            new SearchAddress("Trust Address"),
            new SearchType("MAT", 1),
            null,
            null,
            new SearchCategory("Group"),
            12);
    }

    [Fact]
    public void CanMap_ReturnsTrue_ForGroup()
    {
        Assert.True(_mapper.CanMap(MakeResult()));
    }

    [Fact]
    public void CanMap_ReturnsFalse_ForEstablishment()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("111111"),
                new Name("School"),
                null,
                null,
                null,
                null,
                new SearchCategory("Establishment"),
                0);

        Assert.False(_mapper.CanMap(input));
    }

    [Fact]
    public void Map_SetsCaptionAndLink()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        Assert.Equal("Trust Name", table.Caption);
        Assert.Equal("/groups/222222", table.CaptionLinkUrl);
    }

    [Fact]
    public void Map_AddsGroupIdRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Group ID");

        Assert.Equal("222222", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsTypeRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Type");

        Assert.Equal("MAT", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsAddressRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Address");

        Assert.Equal("Trust Address", row.Cells[1].Text);
    }

    [Fact]
    public void Map_AddsAcademiesRow()
    {
        GovUkTable table = _mapper.Map(MakeResult());

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Academies");

        Assert.Equal("12", row.Cells[1].Text);
    }

    [Fact]
    public void Map_UsesNullAddress_WhenAddressIsNull()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("222222"),
                new Name("Trust"),
                null,
                new SearchType("MAT", 1),
                null,
                null,
                new SearchCategory("Group"),
                5);

        GovUkTable table = _mapper.Map(input);

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Address");

        Assert.Null(row.Cells[1].Text);
    }

    [Fact]
    public void Map_UsesNullType_WhenTypeIsNull()
    {
        SearchAggregateResult input =
            SearchAggregateResult.Create(
                new ProviderIdentifier("222222"),
                new Name("Trust"),
                null,
                null,
                null,
                null,
                new SearchCategory("Group"),
                5);

        GovUkTable table = _mapper.Map(input);

        TableRow row =
            table.Rows.Single(x => x.Cells[0].Text == "Type");

        Assert.Null(row.Cells[1].Text);
    }
}