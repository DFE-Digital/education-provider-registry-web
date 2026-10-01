using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Mappers;

public class UseCaseReponseToViewModelMapper : IMapper<Dataset, DownloadedDatasetViewModel>
{
    public DownloadedDatasetViewModel Map(Dataset input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new DownloadedDatasetViewModel
        {
            Filename = input.Filename
                ?? throw new InvalidOperationException(
                    "Dataset filename was null."),
            ContentType = input.DataType
                ?? throw new InvalidOperationException(
                    "Dataset data type was null."),
            FileFormat = input.DataFormat
                ?? throw new InvalidOperationException(
                    "Dataset data format was null."),
            FileSize = input.FileSize
        };
    }
}

