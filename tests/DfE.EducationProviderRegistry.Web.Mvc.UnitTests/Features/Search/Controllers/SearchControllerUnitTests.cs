using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers;

public sealed class SearchControllerUnitTests
{
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
                facetResultToViewModelMapper: searchFacetsResultsMapper.Object);

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

        EstablishmentSearchResults establishmentResults = EstablishmentSearchResultsStub.Empty();
        SearchFacets facets = SearchFacetsStub.Empty();

        SearchResultsViewModel mappedViewModel =
            SearchResultsViewModelStub.WithEstablishmentResults([]);

        var searchUseCase =
            SearchUseCaseTestDouble.MockFor(
                UseCaseResponseSearchResponseTestDouble.Success(establishmentResults, facets));

        var searchResultsMapper =
            SearchResultsMapperTestDouble.MockFor(mappedViewModel);

        var searchFacetsResultsMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(mappedFilters);

        SearchController sut =
            new(
                searchUseCase: searchUseCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                facetResultToViewModelMapper: searchFacetsResultsMapper.Object);

        // act
        IActionResult result = await sut.Search(model);

        // assert
        searchFacetsResultsMapper.Verify(mapper =>
            mapper.Map(model.SelectedFacets), Times.Once);

        searchUseCase.Verify(useCase =>
            useCase.HandleRequestAsync(
                It.IsAny<SearchRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        searchResultsMapper.Verify(mapper =>
            mapper.Map(It.IsAny<UseCaseResponse<SearchResponse>>()),
            Times.Once);

        Assert.Equal("academy", mappedViewModel.PrimarySearchTerms);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Results", view.ViewName);
        Assert.Equal(mappedViewModel, view.Model);
    }

    [Fact]
    public async Task Search_AllowsNullFacetDictionary()
    {
        // arrange
        SearchRequestViewModel model = SearchRequestViewModelStub.AcademyWithoutFacet();
        ReadOnlyCollection<FilterRequest> mappedFilters = FilterRequestStub.EstablishmentTypeFacet();

        EstablishmentSearchResults establishmentResults = EstablishmentSearchResultsStub.Empty();
        SearchFacets facets = SearchFacetsStub.Empty();

        SearchResultsViewModel mappedViewModel =
            SearchResultsViewModelStub.WithEstablishmentResults([]);

        var searchUseCase =
            SearchUseCaseTestDouble.MockFor(
                UseCaseResponseSearchResponseTestDouble.Success(establishmentResults, facets));

        var searchResultsMapper =
            SearchResultsMapperTestDouble.MockFor(mappedViewModel);

        var searchFacetsResultsMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(mappedFilters);

        SearchController sut =
            new(
                searchUseCase: searchUseCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                facetResultToViewModelMapper: searchFacetsResultsMapper.Object);

        // act
        IActionResult result = await sut.Search(model);

        // assert
        searchFacetsResultsMapper.Verify(mapper => mapper.Map(null), Times.Once);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Results", view.ViewName);
    }
}
