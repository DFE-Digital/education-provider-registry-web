using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed record SearchFiltersMappingContext(
    ReadOnlyCollection<FilterRequest> FilterRequests,
    SearchRequestViewModel SearchRequest,
    UseCaseResponse<SearchResponse> SearchResponse);