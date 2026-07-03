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

public class EstablishmentDetailsBasicDetailsTableMapper :
    IMapper<EstablishmentDetailsModel, GovUkTable>
{
    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create();

        builder.AddRow(new GovUkTableCell { Text = "URN", IsBold = true },
                       new GovUkTableCell { Text = dto.Urn.Value });

        builder.AddRow(new GovUkTableCell { Text = "Number", IsBold = true },
                       new GovUkTableCell { Text = dto.Number.Value });

        builder.AddRow(new GovUkTableCell { Text = "Status", IsBold = true },
                       new GovUkTableCell { Text = dto.Status.Value });

        builder.AddRow(new GovUkTableCell { Text = "Type", IsBold = true },
                       new GovUkTableCell { Text = dto.Type.Value });

        builder.AddRow(new GovUkTableCell { Text = "Phase of education", IsBold = true },
                       new GovUkTableCell { Text = dto.Phase.Value });

        builder.AddRow(new GovUkTableCell { Text = "Open date", IsBold = true },
                       new GovUkTableCell { Text = dto.LifecycleEventOpened?.EventDate.ToShortDateString() });

        builder.AddRow(new GovUkTableCell { Text = "Open reason", IsBold = true },
                       new GovUkTableCell { Text = dto.LifecycleEventOpened?.Reason.Reason });

        builder.AddRow(new GovUkTableCell { Text = "Closed date", IsBold = true },
                       new GovUkTableCell { Text = dto.LifecycleEventClosed?.EventDate.ToShortDateString() });

        builder.AddRow(new GovUkTableCell { Text = "Closed reason", IsBold = true },
                       new GovUkTableCell { Text = dto.LifecycleEventClosed?.Reason.Reason });

        builder.AddRow(new GovUkTableCell { Text = "Uid", IsBold = true },
                       new GovUkTableCell { Text = dto.Uid });

        builder.AddRow(new GovUkTableCell { Text = "Grope name", IsBold = true },
                       new GovUkTableCell { Text = dto.GroupName });

        builder.AddRow(new GovUkTableCell { Text = "Group type", IsBold = true },
                       new GovUkTableCell { Text = dto.GroupType });

        builder.AddRow(new GovUkTableCell { Text = "Group open date", IsBold = true },
                       new GovUkTableCell { Text = dto.GroupOpenDate.ToString() });

        return builder.Build();
    }
}

public class EstablishmentDetailsGovernorsTableMapper :
    IMapper<IEnumerable<GovernorModel>, GovUkTable>
{
    public GovUkTable Map(IEnumerable<GovernorModel> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Governors")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (GovernorModel g in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = g.Name.Value },
                new GovUkTableCell { Text = g.Identifier.Value },
                new GovUkTableCell { Text = string.Empty }
            );
        }

        return builder.Build();
    }
}