using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Extensions;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchTests : WebApplicationFactoryBaseIntegrationTest
{
    public SearchTests(IServiceProvider provider) : base(provider)
    {

    }

    [Fact]
    public async Task Search_With_Empty_Terms_Returns_Error_Message()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        // Assert
        // TODO both terms are empty returns ModelResult.Err
        Assert.Skip("Search with empty terms is not yet implemented");
    }

    [Fact]
    public async Task Search_With_Identity_Term_Returns_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("School")
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();
        // TODO results
    }

    [Fact]
    public async Task Search_With_Location_Term_Returns_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithLocationTerm("London")
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        // TODO SearchResults
    }

    [Fact]
    public async Task Search_With_Identity_And_Location_Returns_Intersecting_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithLocationTerm("London")
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument doc = await response.AssertSuccessfulHtmlResponseAsync();

        // TODO SearchResults
    }

    [Fact]
    public async Task Search_With_Single_Filter_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

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
        SearchFiltersComponent filters = new(document: await response.AssertSuccessfulHtmlResponseAsync());

        // TODO FilteredSearchResults

        // Assert Filter
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
    public async Task Search_With_SingleFilter_And_Multiple_FilterValues_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

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
        SearchFiltersComponent filters = new(document: await response.AssertSuccessfulHtmlResponseAsync());

        // TODO FilteredResults

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
    public async Task Search_With_Multiple_Filters_Returns_Results()
    {
        Assert.Skip("Multiple filters not yet available");
    }

    [Fact]
    public async Task Search_Remove_A_Filter_Removes_Applied_Filter()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

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

        // Assert
        SearchFiltersComponent removedFilters = new(document: await removalResponse.AssertSuccessfulHtmlResponseAsync());
        // TODO FilteredResults

        Filter remainingSelectedFilter = Assert.Single(removedFilters.GetFilters());
        Assert.Equal("Establishment Type", remainingSelectedFilter.Name);

        FilterValue remainingSelectedFilterValue = remainingSelectedFilter.FilterValues.Single();
        Assert.True(remainingSelectedFilterValue.Selected);
        Assert.Equal("2", remainingSelectedFilterValue.Value);
        Assert.NotEmpty(remainingSelectedFilterValue.Label);
    }


    [Fact]
    public async Task Search_Clear_Filter_Removes_All_Applied_Filters()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

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
        SearchFiltersComponent clearedFilters = new(document: await removalResponse.AssertSuccessfulHtmlResponseAsync());
        // TODO FilteredResults

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

    // Results count displayed
    // Sort omitted - A-Z, pass az and Z-A, unknown defaults to az
    // RecordsPerPage = omitted 10, 0 (invalid), 1 valid, 20 valid, 21 invalid
}