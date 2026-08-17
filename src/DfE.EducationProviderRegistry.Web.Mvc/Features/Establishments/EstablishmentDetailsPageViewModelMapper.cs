using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public class EstablishmentDetailsPageViewModelMapper :
    IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>
{
    private readonly IMapper<EstablishmentDetailsModel, GovUkTable> _basicToTableMapper;
    private readonly IMapper<IEnumerable<GovernorModel>, GovUkTable> _governorsToTableMapper;

    public EstablishmentDetailsPageViewModelMapper(
        IMapper<EstablishmentDetailsModel, GovUkTable> basicMapper,
        IMapper<IEnumerable<GovernorModel>, GovUkTable> governorMapper)
    {
        _basicToTableMapper = basicMapper;
        _governorsToTableMapper = governorMapper;
    }

    public EstablishmentDetailsPageViewModel Map(EstablishmentDetailsModel model)
    {
        return new EstablishmentDetailsPageViewModel
        {
            Heading = model.Name.Value,
            BasicDetails = _basicToTableMapper.Map(model),
            Governors = _governorsToTableMapper.Map(model.Governors),
        };
    }
}
