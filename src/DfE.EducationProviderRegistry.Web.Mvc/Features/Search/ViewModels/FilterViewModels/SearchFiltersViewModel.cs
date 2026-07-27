using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.FilterViewModels
{
    public sealed class SearchFiltersViewModel
    {
        public IReadOnlyCollection<FilterViewModel> Filters { get; init; } = [];

        public IReadOnlyCollection<SelectedFilterViewModel> SelectedFilters { get; init; } = [];
    }
}