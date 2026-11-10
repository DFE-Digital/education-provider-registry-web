using System.Net;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Document.Abstractions;
using DfE.Document.Anglesharp;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchPaginationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Normalises_PageNumber_LessThan_1(int pageNumber)
    {
        // Arrange
        UseCaseStub<SearchRequest, SearchResponse> useCase =
            SearchUseCaseTestDoubles.StubFor(
                SearchAggregateResults.CreateEmpty(),
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithPage(pageNumber)
                .Build();

        // Act
        using HttpResponseMessage response = await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Offset calculates from PageSize and passes to UseCase.
        // Page 1 offsets no results
        Assert.Equal(0, useCase.ReceivedRequest!.Offset);

    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 10)]
    [InlineData(3, 20)]
    public async Task Maps_Page_Number_To_Search_Request_Offset(int pageNumber, int expectedOffset)
    {
        // Arrange
        UseCaseStub<SearchRequest, SearchResponse> useCase =
            SearchUseCaseTestDoubles.StubFor(
                SearchAggregateResults.CreateEmpty(),
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithPage(pageNumber)
                .Build();

        // Act
        await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(useCase.ReceivedRequest);
        Assert.Equal(expectedOffset, useCase.ReceivedRequest.Offset);

        const int defaultPageSize = 10;
        Assert.Equal(defaultPageSize, useCase.ReceivedRequest.PageSize);
    }

    [Fact]
    public async Task Does_Not_Display_Pagination_When_Only_One_Page_Exists()
    {
        // Arrange
        const int defaultPageSize = 10;
        SearchAggregateResults searchResults = SearchAggregateResultsTestDouble.Stub(defaultPageSize);

        UseCaseStub<SearchRequest, SearchResponse> useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults: searchResults,
                facets: SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client =
            factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .Build();

        // Act
        HttpResponseMessage response =
            await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await response.AssertSuccessfulHtmlResponseAsync();

        IQueryHandler query = new QueryHandler(SearchContext.From(document));

        GovUkPaginationComponent paginationComponent = new(query);

        Assert.Null(paginationComponent.Pages());
    }

    [Fact]
    public async Task Displays_Pagination_When_Multiple_Pages_Exist()
    {
        // Arrange
        SearchAggregateResults searchResults = SearchAggregateResultsTestDouble.Stub(11);

        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults,
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .Build();

        // Act
        HttpResponseMessage response =
            await client.SendAsync(
                message,
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await response.AssertSuccessfulHtmlResponseAsync();

        IQueryHandler query = new QueryHandler(SearchContext.From(document));

        GovUkPaginationComponent paginationComponent = new(query);

        Assert.NotNull(paginationComponent.Pages());
    }

    [Fact]
    public async Task Does_Not_Display_Previous_Link_On_First_Page()
    {
        // Arrange
        SearchAggregateResults searchResults = SearchAggregateResultsTestDouble.Stub(11);

        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults,
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithPage(1)
                .Build();

        // Act
        HttpResponseMessage response =
            await client.SendAsync(message, TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await response.AssertSuccessfulHtmlResponseAsync();

        IQueryHandler query = new QueryHandler(SearchContext.From(document));

        GovUkPaginationComponent paginationComponent = new(query);

        Assert.Null(paginationComponent.Pages()!.Previous);
    }

    [Fact]
    public async Task Does_Not_Display_Next_Link_On_Last_Page()
    {
        // Arrange
        SearchAggregateResults searchResults = SearchAggregateResultsTestDouble.Stub(30);

        IUseCase<SearchRequest, UseCaseResponse<SearchResponse>> useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults,
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(useCase);

        using HttpClient client = factory.CreateClient();

        using HttpRequestMessage message =
            SearchHttpRequestBuilder.Create()
                .WithBaseUri(factory.Server.BaseAddress)
                .WithIdentitySearchTerm("sch")
                .WithPage(3)
                .Build();

        // Act
        HttpResponseMessage response =
            await client.SendAsync(
                message,
                TestContext.Current.CancellationToken);

        // Assert
        IHtmlDocument document =
            await response.AssertSuccessfulHtmlResponseAsync();

        IQueryHandler query = new QueryHandler(SearchContext.From(document));

        GovUkPaginationComponent paginationComponent = new(query);

        Assert.Equal(3, paginationComponent.Pages()!.Current);
        Assert.NotNull(paginationComponent.Pages()!.Previous);
        Assert.Null(paginationComponent.Pages()!.Next);
    }
}
