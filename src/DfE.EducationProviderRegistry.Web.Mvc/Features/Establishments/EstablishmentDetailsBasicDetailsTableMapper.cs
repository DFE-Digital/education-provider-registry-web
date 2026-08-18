using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public class EstablishmentDetailsBasicDetailsTableMapper :
    IMapper<EstablishmentDetailsModel, GovUkTable>
{
    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create();

        builder.AddRow(new TableCell { Text = "URN", IsBold = true },
                       new TableCell { Text = dto.Urn.Value });

        builder.AddRow(new TableCell { Text = "Number", IsBold = true },
                       new TableCell { Text = dto.Number.Value ?? "" });

        builder.AddRow(new TableCell { Text = "Status", IsBold = true },
                       new TableCell { Text = dto.Status.Value ?? "" });

        builder.AddRow(new TableCell { Text = "Type", IsBold = true },
                       new TableCell { Text = dto.Type.Value });

        builder.AddRow(new TableCell { Text = "Phase of education", IsBold = true },
                       new TableCell { Text = dto.Phase.Value ?? "" });

        builder.AddRow(new TableCell { Text = "Open date", IsBold = true },
                       new TableCell { Text = dto.LifecycleEventOpened?.EventDate.ToShortDateString() ?? "" });

        builder.AddRow(new TableCell { Text = "Open reason", IsBold = true },
                       new TableCell { Text = dto.LifecycleEventOpened?.Reason.Reason ?? "" });

        builder.AddRow(new TableCell { Text = "Closed date", IsBold = true },
                       new TableCell { Text = dto.LifecycleEventClosed?.EventDate.ToShortDateString() ?? "" });

        builder.AddRow(new TableCell { Text = "Closed reason", IsBold = true },
                       new TableCell { Text = dto.LifecycleEventClosed?.Reason.Reason ?? "" });

        builder.AddRow(new TableCell { Text = "Uid", IsBold = true },
                       new TableCell { Text = dto.Uid ?? "" });

        builder.AddRow(new TableCell { Text = "Grope name", IsBold = true },
                       new TableCell { Text = dto.GroupName ?? "" });

        builder.AddRow(new TableCell { Text = "Group type", IsBold = true },
                       new TableCell { Text = dto.GroupType ?? "" });

        builder.AddRow(new TableCell { Text = "Group open date", IsBold = true },
                       new TableCell { Text = dto.GroupOpenDate.ToString() ?? "" });

        return builder.Build();
    }
}
