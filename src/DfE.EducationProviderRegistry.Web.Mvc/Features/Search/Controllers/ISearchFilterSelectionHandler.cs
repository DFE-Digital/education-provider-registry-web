using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;

public interface ISearchFilterSelectionHandler
{
    void Handle(SearchRequestViewModel request);
}