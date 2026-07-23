
namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels
{
    public sealed class SearchFiltersViewModel
    {
        public IReadOnlyCollection<FilterViewModel> Filters { get; init; } = [];
    }
}