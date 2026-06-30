using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Mappers;

public sealed class SelectedFacetsToFilterRequestsMapper :
    IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>
{
    public ReadOnlyCollection<FilterRequest> Map(Dictionary<string, List<string>>? selectedFacets)
    {
        // No filters posted → return empty list
        if (selectedFacets is null || selectedFacets.Count == 0)
        {
            return [];
        }

        // Convert dictionary → List<FilterRequest>
        List<FilterRequest> mapped =
        [
            .. selectedFacets.Select(kvp =>
                new FilterRequest(
                    filterName: kvp.Key,
                    filterValues: [.. kvp.Value.Cast<object>()]))
        ];

        return mapped.AsReadOnly();
    }
}
