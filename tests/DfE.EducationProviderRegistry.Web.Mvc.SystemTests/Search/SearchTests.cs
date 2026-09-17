using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests.Search;

public sealed class SearchTests : WebApplicationFactoryBaseTest
{
    public SearchTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task Apply_A_Filter_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        // TODO stub UseCaseResponse with results
        using HttpClient client = Factory.CreateClient();

        string[] filterValueApplied = ["1"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", filterValueApplied)
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();
        SearchFiltersComponent filters = new(document);
        SearchResultsComponent results = new(document);

        Assert.NotEmpty(results.GetSearchResults());

        // Only selected filters are displayed
        Filter selected = Assert.Single(filters.GetFilters());
        Assert.Equal("Establishment Type", selected.Name);

        // Assert FilterValue
        FilterValue value = Assert.Single(selected.FilterValues);

        Assert.True(value.Selected);
        Assert.NotEmpty(value.Label);
        Assert.Equal(filterValueApplied.Single(), value.Value);
    }

    [Fact]
    public async Task Apply_A_Filter_With_Multiple_FilterValues_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        // TODO stub UseCaseResponse with results
        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["1", "2"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", filtersToApply)
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();
        SearchFiltersComponent filters = new(document);
        SearchResultsComponent results = new(document);

        Assert.NotEmpty(results.GetSearchResults());

        // Assert Filter
        Filter selected = Assert.Single(filters.GetFilters());
        Assert.Equal("Establishment Type", selected.Name);

        // Assert FilterValues
        Assert.Equal(2, selected.FilterValues.Count);

        FilterValue value1 = selected.FilterValues.Single(v => v.Value == filtersToApply[0]);
        Assert.True(value1.Selected);
        Assert.NotEmpty(value1.Label);

        FilterValue value2 = selected.FilterValues.Single(v => v.Value == filtersToApply[1]);
        Assert.True(value2.Selected);
        Assert.NotEmpty(value2.Label);
    }

    [Fact]
    public async Task Apply_Multiple_Filters_Returns_Filtered_Results()
    {
        Assert.Skip("Multiple filters not yet available");
    }


    [Fact]
    public async Task Remove_A_Filter_Returns_Remaining_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        // TODO stub UseCaseResponse with results
        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["1", "2"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", filtersToApply)
                .Build();

        HttpResponseMessage filteredResults = await client.SendAsync(message, ct);
        SearchFiltersComponent filtersApplied = new(document: await filteredResults.AssertSuccessfulHtmlResponseAsync());

        // Act
        HttpResponseMessage removalResponse =
            await filtersApplied.RemoveFilterAsync(
                client,
                facetLabel: "EstablishmentTypeId",
                facetValue: "1",
                ct);

        IHtmlDocument document = await removalResponse.AssertSuccessfulHtmlResponseAsync();

        // Assert
        SearchFiltersComponent removedFilters = new(document);
        SearchResultsComponent results = new(document);

        Assert.NotEmpty(results.GetSearchResults());
        Filter remainingSelectedFilter = Assert.Single(removedFilters.GetFilters());
        Assert.Equal("Establishment Type", remainingSelectedFilter.Name);

        FilterValue remainingSelectedFilterValue = remainingSelectedFilter.FilterValues.Single();
        Assert.True(remainingSelectedFilterValue.Selected);
        Assert.Equal("2", remainingSelectedFilterValue.Value);
        Assert.NotEmpty(remainingSelectedFilterValue.Label);
    }

    [Fact]
    public async Task Clear_Filters_Removes_Applied_Filters()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        // TODO stub UseCaseResponse with results
        using HttpClient client = Factory.CreateClient();

        string[] filtersToApply = ["1", "2"];

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", filtersToApply)
                .Build();

        HttpResponseMessage filteredResults = await client.SendAsync(message, ct);
        SearchFiltersComponent filtersApplied = new(document: await filteredResults.AssertSuccessfulHtmlResponseAsync());

        // Act
        HttpResponseMessage removalResponse = await filtersApplied.ClearFiltersAsync(client, ct);

        // Assert
        IHtmlDocument document = await removalResponse.AssertSuccessfulHtmlResponseAsync();
        SearchFiltersComponent clearedFilters = new(document);
        SearchResultsComponent results = new(document);

        Assert.NotEmpty(results.GetSearchResults());

        Filter filters = Assert.Single(clearedFilters.GetFilters());
        Assert.Equal("Establishment Type", filters.Name);

        // static 2 filter values in data. None selected
        FilterValue value1 = filters.FilterValues.Single(v => v.Value == filtersToApply[0]);
        Assert.False(value1.Selected);
        Assert.NotEmpty(value1.Label);

        FilterValue value2 = filters.FilterValues.Single(v => v.Value == filtersToApply[1]);
        Assert.False(value2.Selected);
        Assert.NotEmpty(value2.Label);
    }
}
