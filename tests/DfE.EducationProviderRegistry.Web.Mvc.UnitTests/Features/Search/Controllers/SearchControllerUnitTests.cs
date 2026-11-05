using System.Collections.ObjectModel;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
                selectedFiltersToFiltersRequestsMapper: SearchFiltersToFilterRequestMapperTestDouble.Mock().Object,
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
                selectedFiltersToFiltersRequestsMapper: SearchFiltersToFilterRequestMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Throws_When_FiltersToFilterReuqestsMapperIsNull()
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
                selectedFiltersToFiltersRequestsMapper: SearchFiltersToFilterRequestMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: null!);

        // act & assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Index_ReturnsCorrectViewAndModel()
    {
        // arrange
        Mock<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>> useCase = SearchUseCaseTestDouble.Mock();
        Mock<IMapper<SearchResultsMappingContext, SearchResultsViewModel>> searchResultsMapper = SearchResultsMapperTestDouble.Mock();
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> searchFacetsResultsMapper = SearchFacetsResultsMapperTestDouble.Mock();

        SearchController sut =
            new(
                searchUseCase: useCase.Object,
                searchResponseToViewModelMapper: searchResultsMapper.Object,
                selectedFiltersToFiltersRequestsMapper: SearchFiltersToFilterRequestMapperTestDouble.Mock().Object,
                searchFilterSelectionHandler: SearchFilterSelectionHandlerStub.Mock().Object);

        // act
        IActionResult result = sut.Index();

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.IsType<SearchRequestViewModel>(view.Model);
    }

    [Fact]
    public async Task Search_DoesNotInvokeDependencies_WhenModelStateIsInvalid()
    {
        // arrange
        Mock<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>> useCase = SearchUseCaseTestDouble.Mock();
        Mock<IMapper<SearchResultsMappingContext, SearchResultsViewModel>> mapper = SearchResultsMapperTestDouble.Mock();
        Mock<IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>>> filtersMapper = SearchFiltersToFilterRequestMapperTestDouble.Mock();
        Mock<ISearchFilterSelectionHandler> selectionHandler = SearchFilterSelectionHandlerStub.Mock();

        SearchController sut =
            new(
                searchUseCase: useCase.Object,
                searchResponseToViewModelMapper: mapper.Object,
                selectedFiltersToFiltersRequestsMapper: filtersMapper.Object,
                searchFilterSelectionHandler: selectionHandler.Object);

        sut.ModelState.AddModelError(
            "What",
            "Validation error");

        // act
        await sut.Search(new SearchRequestViewModel());

        // assert
        useCase.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
        filtersMapper.VerifyNoOtherCalls();
        selectionHandler.VerifyNoOtherCalls();
    }
}
