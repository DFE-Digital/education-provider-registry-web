namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

public sealed class DownloadedDatasetViewModel
{
    public required string Filename { get; init; }

    // Appends the current date (yyMMdd) and ensures the .zip extension is at the end
    public string ZipFilename
    {
        get
        {
            string dateSuffix = DateTime.UtcNow.ToString("yyMMdd");
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(Filename);
            string extension = Path.GetExtension(Filename);

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".zip";
            }

            return $"{nameWithoutExtension}_{dateSuffix}{extension}";
        }
    }

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
                return $"{FileSize / KB:F2} KB";
            }

            return $"{FileSize / MB:F2} MB";
        }
    }
}
