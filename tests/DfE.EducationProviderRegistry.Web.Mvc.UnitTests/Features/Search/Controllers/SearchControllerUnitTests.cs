using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers;

public sealed class SearchControllerUnitTests
{
    [Fact]
    public void Constructor_Throws_When_SearchUseCaseIsNull()
    {
        // arrange
        Func<SearchController> construct = () =>
            new(
                searchUseCase: null!,
                searchResponseToViewModelMapper: SearchResultsMapperTestDouble.Mock().Object,
                selectedFacetsToFilterRequestsMapper: SearchFacetsResultsMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_SearchResponseToViewModelMapperIsNull()
    {
        // arrange
        Func<SearchController> construct = () =>
            new(
                searchUseCase: SearchUseCaseTestDouble.Mock().Object,
                searchResponseToViewModelMapper: null!,
                selectedFacetsToFilterRequestsMapper: SearchFacetsResultsMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_FacetResultToViewModelMapperIsNull()
    {
        // arrange
        Func<SearchController> construct = () =>
            new(
                searchUseCase: SearchUseCaseTestDouble.Mock().Object,
                searchResponseToViewModelMapper: SearchResultsMapperTestDouble.Mock().Object,
                null!,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_SearchFilterSelectionHandlerIsNull()
    {
        // arrange
        Func<SearchController> construct = () =>
            new(
                searchUseCase: SearchUseCaseTestDouble.Mock().Object,
                searchResponseToViewModelMapper: SearchResultsMapperTestDouble.Mock().Object,
                selectedFacetsToFilterRequestsMapper: SearchFacetsResultsMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: null!);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Index_ReturnsCorrectViewAndModel()
    {
        // arrange
        var useCase = SearchUseCaseTestDouble.Mock();
        var searchResultsMapper = SearchResultsMapperTestDouble.Mock();
        var searchFacetsResultsMapper = SearchFacetsResultsMapperTestDouble.Mock();

        SearchController sut =
            new(
                searchUseCase: useCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                selectedFacetsToFilterRequestsMapper: searchFacetsResultsMapper.Object,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act
        IActionResult result = sut.Index();

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.IsType<SearchRequestViewModel>(view.Model);
    }

    [Fact]
    public async Task Search_MapsFacets_InvokesUseCase_MapsResponse_ReturnsCorrectView()
    {
        // arrange
        SearchRequestViewModel model = SearchRequestViewModelStub.AcademyWithFacet();
        ReadOnlyCollection<FilterRequest> mappedFilters = FilterRequestStub.EstablishmentTypeFacet();

        SearchAggregateResults searchAggregateResults = SearchAggregateResultsStub.Empty();
        SearchFacets facets = SearchFacetsStub.Empty();

        SearchResultsViewModel mappedViewModel =
            SearchResultsViewModelStub.WithEstablishmentResults([]);

        Mock<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>> searchUseCase =
            SearchUseCaseTestDouble.MockFor(
                UseCaseResponseSearchResponseTestDouble.Success(searchAggregateResults, facets));

        Mock<IMapper<SearchResultsMappingContext, SearchResultsViewModel>> searchResultsMapper =
            SearchResultsMapperTestDouble.MockFor(mappedViewModel);

        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> searchFacetsResultsMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(mappedFilters);

        Mock<ISearchFilterSelectionHandler> searchFilterSelectionHandler = SearchFilterSelectionHandlerStub.MockFor();

        SearchController sut =
            new(
                searchUseCase: searchUseCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                selectedFacetsToFilterRequestsMapper: searchFacetsResultsMapper.Object,
                searchFilterSelectionHandler: searchFilterSelectionHandler.Object);

        // act
        IActionResult result = await sut.Search(model);

        // assert
        searchFacetsResultsMapper.Verify(mapper =>
            mapper.Map(model.SelectedFacets),
            Times.Once);

        searchUseCase.Verify(useCase =>
            useCase.HandleRequestAsync(
                It.IsAny<SearchRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        searchResultsMapper.Verify(mapper =>
            mapper.Map(It.Is<SearchResultsMappingContext>(context =>
                context.SearchRequest.SearchKeywords == "academy")),
            Times.Once);

        searchFilterSelectionHandler.Verify(handler =>
            handler.Handle(model),
            Times.Once);

        ViewResult view = Assert.IsType<ViewResult>(result);

        Assert.Equal("Results", view.ViewName);
        Assert.Same(mappedViewModel, view.Model);
    }

    [Fact]
    public async Task Search_AllowsNullFacetDictionary()
    {
        // arrange
        SearchRequestViewModel model = SearchRequestViewModelStub.AcademyWithoutFacet();
        ReadOnlyCollection<FilterRequest> mappedFilters = FilterRequestStub.EstablishmentTypeFacet();

        SearchAggregateResults searchAggregateResults = SearchAggregateResultsStub.Empty();
        SearchFacets facets = SearchFacetsStub.Empty();

        SearchResultsViewModel mappedViewModel =
            SearchResultsViewModelStub.WithEstablishmentResults([]);

        Mock<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>> searchUseCase =
            SearchUseCaseTestDouble.MockFor(
                UseCaseResponseSearchResponseTestDouble.Success(searchAggregateResults, facets));

        Mock<IMapper<SearchResultsMappingContext, SearchResultsViewModel>> searchResultsMapper =
            SearchResultsMapperTestDouble.MockFor(mappedViewModel);

        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> searchFacetsResultsMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(mappedFilters);


        Mock<ISearchFilterSelectionHandler> searchFilterSelectionHandler = SearchFilterSelectionHandlerStub.MockFor();

        SearchController sut =
            new(
                searchUseCase: searchUseCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                selectedFacetsToFilterRequestsMapper: searchFacetsResultsMapper.Object,
                searchFilterSelectionHandler: searchFilterSelectionHandler.Object);

        // act
        IActionResult result = await sut.Search(model);

        // assert
        searchFacetsResultsMapper.Verify(mapper => mapper.Map(new()), Times.Once);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Results", view.ViewName);
    }
}
