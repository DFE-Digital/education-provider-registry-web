using AngleSharp.Html.Dom;

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

        // Assert
        // TODO both terms are empty returns ModelResult.Err
        Assert.Skip("Search with empty terms is not yet implemented");
        
        // response.EnsureSuccessStatusCode();
        // Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
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
        response.EnsureSuccessStatusCode();
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

        response.EnsureSuccessStatusCode();
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

        response.EnsureSuccessStatusCode();
        // TODO SearchResults
    }

    [Fact]
    public async Task Search_With_Single_Filter_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        const string filterValueApplied = "1";

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", [filterValueApplied])
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        response.EnsureSuccessStatusCode();

        IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(response);

        SearchFilters filters = new(document);

        // TODO FilteredResults

        // Assert Filter
        // Only selected filters are displayed
        Filter selected = Assert.Single(filters.GetFilters()); 
        Assert.Equal("Establishment Type", selected.Name);

        // Assert FilterValue
        FilterValue value = Assert.Single(selected.FilterValues);
        Assert.True(value.Selected);
        Assert.NotEmpty(value.Label);
        Assert.Equal(filterValueApplied, value.Value);
    }

    [Fact]
    public async Task Search_With_SingleFilter_And_Multiple_FilterValues_Returns_Filtered_Results()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;
        using HttpClient client = Factory.CreateDefaultedHttpClient();

        const string filterValueApplied1 = "1";
        const string filterValueApplied2 = "2";

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithFilter("EstablishmentTypeId", [filterValueApplied1, filterValueApplied2])
                .Build();
        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);
        
        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(response);
        SearchFilters filters = new(document);

        // TODO FilteredResults

        // Assert Filter
        Filter selected = Assert.Single(filters.GetFilters());
        Assert.Equal("Establishment Type", selected.Name);
        
        // Assert FilterValues
        Assert.Equal(2, selected.FilterValues.Count);

        FilterValue value1 = selected.FilterValues.Single(v => v.Value == filterValueApplied1);
        Assert.True(value1.Selected);
        Assert.NotEmpty(value1.Label);

        FilterValue value2 = selected.FilterValues.Single(v => v.Value == filterValueApplied2);
        Assert.True(value2.Selected);
        Assert.NotEmpty(value2.Label);
    }

    // TODO apply different filters (EstablishmentType, Status)

    // TODO remove specific filter
    // TODO clear filters

    // Results count displayed
    // Sort omitted - A-Z, pass az and Z-A, unknown defaults to az
    // RecordsPerPage = omitted 10, 0 (invalid), 1 valid, 20 valid, 21 invalid
}

