using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.ViewModels;

public sealed class DownloadedDatasetViewModelTests
{
    [Fact]
    public void ZipFilename_AppendsZipExtension()
    {
        // arrange
        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "myfile",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = 100
            };

        // assert
        Assert.Equal("myfile.zip", vm.ZipFilename);
    }

    [Fact]
    public void FormattedFileSize_ReturnsKB_WhenUnder1MB()
    {
        // arrange
        const long fileSize = 500_000; // ~488 KB

        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = fileSize
            };

        // act
        string formatted = vm.FormattedFileSize;

        // assert
        Assert.EndsWith("KB", formatted);
        Assert.Equal($"{fileSize / 1024d:F2} KB", formatted);
    }

    [Fact]
    public void FormattedFileSize_ReturnsMB_WhenOver1MB()
    {
        // arrange
        const long fileSize = 2_500_000; // ~2.38 MB

        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = fileSize
            };

        // act
        string formatted = vm.FormattedFileSize;

        // assert
        Assert.EndsWith("MB", formatted);
        Assert.Equal($"{fileSize / (1024d * 1024d):F2} MB", formatted);
    }

    [Fact]
    public void FormattedFileSize_ZeroBytes_ReturnsZeroKB()
    {
        // arrange
        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = 0
            };

        // assert
        Assert.Equal("0.00 KB", vm.FormattedFileSize);
    }

    [Fact]
    public void FormattedFileSize_Exactly1MB_ReturnsMB()
    {
        // arrange
        const long oneMb = 1024L * 1024L;

        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = oneMb
            };

        // assert
        Assert.Equal("1.00 MB", vm.FormattedFileSize);
    }

    [Fact]
    public void FormattedFileSize_JustUnder1MB_ReturnsKB()
    {
        // arrange
        const long size = (1024L * 1024L) - 1; // 1MB - 1 byte

        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = size
            };

        // assert
        Assert.EndsWith("KB", vm.FormattedFileSize);
        Assert.Equal($"{size / 1024d:F2} KB", vm.FormattedFileSize);
    }

    [Fact]
    public void FormattedFileSize_JustOver1MB_ReturnsMB()
    {
        // arrange
        const long size = (1024L * 1024L) + 1; // 1MB + 1 byte

        DownloadedDatasetViewModel vm =
            new()
            {
                Filename = "file",
                ContentType = "text/csv",
                FileFormat = "csv",
                FileSize = size
            };

        // assert
        Assert.EndsWith("MB", vm.FormattedFileSize);
        Assert.Equal($"{size / (1024d * 1024d):F2} MB", vm.FormattedFileSize);
    }
}
