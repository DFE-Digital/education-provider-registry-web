using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.SharedTests.AngleSharp.Extensions;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search.Components;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests.Search;

public sealed class SearchResultsTests : WebApplicationFactoryBaseSystemTest
{
    public SearchResultsTests(IServiceProvider provider) : base(provider)
    {
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddSearchUseCaseConfiguration((builder) =>
        {
            builder
                .WithSearchTerm(
                    "what",
                    (field) =>
                        field.WithFieldName(nameof(SearchAggregate.ProviderName))
                            .AppendContainsMatchBehaviour())
                .WithSearchTerm(
                    "where",
                    (field) =>
                        field.WithFieldName(nameof(SearchAggregate.Postcode))
                            .AppendStartsWithMatchBehaviour());
        });
    }

    [Fact]
    public async Task Results_Returned_When_Search_By_What_Term()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate match =
            SearchAggregateBuilder.Create()
                .WithProviderName("school 1")
                    .Build();

        SearchAggregate doesNotMatch =
            SearchAggregateBuilder.Create()
                .WithProviderName("College")
                    .Build();

        SearchAggregate[] seed = [match, doesNotMatch];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        using HttpClient client = Factory.CreateClient();

        HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(Factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .Build();

        // Act
        HttpResponseMessage response = await client.SendAsync(message, ct);

        // Assert
        IHtmlDocument document = await response.AssertSuccessfulHtmlResponseAsync();

        SearchResultsComponent resultsComponent = new(document);
        SearchResult displayedResult = Assert.Single(resultsComponent.GetSearchResults());
        Assert.Equal(match.ProviderName, displayedResult.Name);
    }
}
