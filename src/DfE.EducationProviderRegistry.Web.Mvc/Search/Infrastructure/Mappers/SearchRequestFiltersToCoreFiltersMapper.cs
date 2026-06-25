using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

public sealed class SearchRequestFiltersToCoreFiltersMapper :
    IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>
{
    public ReadOnlyCollection<SearchFilterRequest> Map(ReadOnlyCollection<FilterRequest> input)
    {
        if (input == null || input.Count == 0)
        {
            return [];
        }

        List<SearchFilterRequest> results = new(input.Count);

        foreach (FilterRequest filter in input)
        {
            if (filter.FilterValues == null || filter.FilterValues.Count == 0)
            {
                continue;
            }

            // Copy values to avoid leaking the read-only wrapper
            object[] copiedValues = [.. filter.FilterValues];

            SearchFilterRequest mapped =
                new(
                    filterKey: filter.FilterName,
                    filterValues: copiedValues);

            results.Add(mapped);
        }

        return results.AsReadOnly();
    }
}