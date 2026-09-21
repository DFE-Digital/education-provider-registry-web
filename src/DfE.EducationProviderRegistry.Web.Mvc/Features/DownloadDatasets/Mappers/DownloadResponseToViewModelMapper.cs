using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.Mappers;

public sealed class DownloadResponseToViewModelMapper :
    IMapper<DownloadDatasetsResponse, DownloadedDatasetViewModel>
{
    public DownloadedDatasetViewModel Map(DownloadDatasetsResponse input)
    {
        throw new NotImplementedException();
    }
}
