using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public static class EstablishmentServiceCollectionExtensions
{
    public static IServiceCollection AddEstablishments(this IServiceCollection services)
    {
        services.AddTransient<
            IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>,
            EstablishmentDetailsPageViewModelMapper>();
        services.AddTransient<
            IMapper<EstablishmentDetailsModel, GovUkTable>,
            EstablishmentDetailsBasicDetailsTableMapper>();
        services.AddTransient<
            IMapper<IEnumerable<GovernorModel>, GovUkTable>,
            EstablishmentDetailsGovernorsTableMapper>();

        services.AddEstablishmentsUseCaseDependencies();
        services.AddEstablishmentsInfrastructureDependencies();

        return services;
    }
}
