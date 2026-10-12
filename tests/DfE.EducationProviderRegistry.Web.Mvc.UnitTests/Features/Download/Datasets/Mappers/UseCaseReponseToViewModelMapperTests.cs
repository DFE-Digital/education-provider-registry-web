using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Datasets.Mappers;

public sealed class UseCaseResponseToViewModelMapperTests
{
    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        UseCaseReponseToViewModelMapper mapper = new();

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));

        // assert
        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenFilenameIsNull()
    {
        // arrange
        byte[] fileBytes = [];

        Dataset dataset =
            new(
                Filename: null!,
                DataType: "All Establishments",
                DataFormat: "CSV",
                FileSize: 123,
                FileStream: new MemoryStream(fileBytes)
            );

        // act
        UseCaseReponseToViewModelMapper mapper = new();

        // assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => mapper.Map(dataset));

        Assert.Equal("Dataset filename was null.", exception.Message);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenDataTypeIsNull()
    {
        // arrange
        byte[] fileBytes = [];

        Dataset dataset =
            new(
                Filename: "test-dataset",
                DataType: null!,
                DataFormat: "CSV",
                FileSize: 123,
                FileStream: new MemoryStream(fileBytes)
            );

        // act
        UseCaseReponseToViewModelMapper mapper = new();

        // assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => mapper.Map(dataset));

        Assert.Equal("Dataset data type was null.", exception.Message);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenDataFormatIsNull()
    {
        // arrange
        byte[] fileBytes = [];

        Dataset dataset =
            new(
                Filename: "test-dataset",
                DataType: "All Establishments",
                DataFormat: null!,
                FileSize: fileBytes.Length,
                FileStream: new MemoryStream(fileBytes)
            );

        // act
        UseCaseReponseToViewModelMapper mapper = new();

        // assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => mapper.Map(dataset));

        Assert.Equal("Dataset data format was null.", exception.Message);
    }

    [Fact]
    public void Map_ReturnsCorrectViewModel_WhenInputIsValid()
    {
        // arrange
        byte[] fileBytes = [1, 2, 3];

        Dataset dataset =
            new(
                Filename: "test-dataset",
                DataType: "All Establishments",
                DataFormat: "CSV",
                FileSize: 123,
                FileStream: new MemoryStream(fileBytes)
            );

        UseCaseReponseToViewModelMapper mapper = new();

        // act
        DownloadedDatasetViewModel result = mapper.Map(dataset);

        // assert
        Assert.Equal("test-dataset", result.Filename);
        Assert.Equal("All Establishments", result.ContentType);
        Assert.Equal("CSV", result.FileFormat);
        Assert.Equal(123, result.FileSize);
    }
}
