namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

public sealed class DownloadedDatasetViewModel
{
    public required string Filename { get; init; }
    public string ZipFilename => Filename + ".zip";
    public required string ContentType { get; init; }
    public required string FileFormat { get; init; }
    public required long FileSize { get; init; }

    public string FormattedFileSize
    {
        get
        {
            const double KB = 1024d;
            const double MB = KB * 1024d;

            if (FileSize < MB)
            {
                double kb = FileSize / KB;
                return $"{kb:F2} KB";
            }
            else
            {
                double mb = FileSize / MB;
                return $"{mb:F2} MB";
            }
        }
    }
}
