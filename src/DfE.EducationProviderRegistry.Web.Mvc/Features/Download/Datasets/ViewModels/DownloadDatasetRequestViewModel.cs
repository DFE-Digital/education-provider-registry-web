using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

public sealed class DownloadDatasetRequestViewModel
{
    [Required(ErrorMessage = "A download file name is required.")]
    public string? Filename { get; set; }
}
