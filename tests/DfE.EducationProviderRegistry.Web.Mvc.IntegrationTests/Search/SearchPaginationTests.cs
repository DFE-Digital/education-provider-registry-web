using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;
using Docker.DotNet.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchPaginationTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 10)]
    [InlineData(3, 20)]
    public async Task Maps_Page_Number_To_Search_Request_Offset(int pageNumber, int expectedOffset)
    {
        // Arrange
        StubSearchUseCase useCase =
            SearchUseCaseTestDoubles.StubFor(
                SearchAggregateResults.CreateEmpty(),
                SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

        using HttpClient client =
            factory.CreateClient();

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

        StubSearchUseCase useCase =
            SearchUseCaseTestDoubles.StubFor(
                searchResults: searchResults,
                facets: SearchFacetsTestDouble.StubEmpty());

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

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

        PaginationComponent paginationComponent = new(document);

        Assert.False(paginationComponent.Displayed());
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

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

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

        PaginationComponent paginationComponent = new(document);

        Assert.True(paginationComponent.Displayed());
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

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

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

        PaginationComponent paginationComponent = new(document);

        Assert.False(paginationComponent.DisplaysPreviousLink());
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

        using WebApplicationFactory<Program> factory = SearchWebApplicationFactoryProvider.CreateFactory(useCase);

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

        PaginationComponent paginationComponent = new(document);

        Assert.Equal("3", paginationComponent.GetCurrentPage());
        Assert.True(paginationComponent.DisplaysPreviousLink());
        Assert.False(paginationComponent.DisplaysNextLink());
    }
}