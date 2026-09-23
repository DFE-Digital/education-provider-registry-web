using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download;

public static class DownloadServiceCollectionExtensions
{
    public static IServiceCollection AddDownloadDatasets(this IServiceCollection services)
    {
        services
            .AddSingleton<IMapper<
                Dataset, DownloadedDatasetViewModel>,
                UseCaseReponseToViewModelMapper>();

        services.AddDownloadDatasetDependencies();

        return services;
    }

}
