using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels.Validation;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

public sealed class DownloadDatasetRequestViewModel
{
    [NonEmptyList("Choose at least one dataset file download name.")]
    public List<string> SelectedDatasets { get; set; } = [];
}
