using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download;

public class DownloadServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDownloadDatasets_ShouldRegisterMapperAsSingleton()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();

        // act
        IServiceCollection result = services.AddDownloadDatasets();

        // assert
        Assert.Same(services, result);

        ServiceDescriptor? descriptor =
            services.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType == typeof(IMapper<Dataset, DownloadedDatasetViewModel>));

        Assert.NotNull(descriptor);
        Assert.Equal(typeof(UseCaseReponseToViewModelMapper), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddDownloadDatasets_ShouldExecuteSuccessfully()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // act/assert
        Exception? exception =
            Record.Exception(() => services.AddDownloadDatasets());
        Assert.Null(exception);
    }
}
