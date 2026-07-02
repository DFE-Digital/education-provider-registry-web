using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class UseCaseResponseSearchResponseTestDouble
{
    public static UseCaseResponse<SearchResponse> Success(
        EstablishmentSearchResults establishmentResults,
        SearchFacets? facets = null)
    {
        SearchResponse response = new(establishmentResults, facets);
        return UseCaseResponse<SearchResponse>.Success(response);
    }

    public static UseCaseResponse<SearchResponse> EmptySuccess()
    {
        EstablishmentSearchResults emptyResults = new([]);
        SearchFacets emptyFacets = new([]);
        SearchResponse response = new(emptyResults, emptyFacets);

        return UseCaseResponse<SearchResponse>.Success(response);
    }

    public static UseCaseResponse<SearchResponse> Failure(string errorMessage) =>
        UseCaseResponse<SearchResponse>.Failure(errorMessage);
}