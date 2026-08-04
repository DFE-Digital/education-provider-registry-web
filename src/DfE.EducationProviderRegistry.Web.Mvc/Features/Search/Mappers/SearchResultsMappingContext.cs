using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed record SearchResultsMappingContext(
    SearchRequestViewModel SearchRequest,
    UseCaseResponse<SearchResponse> SearchResponse);