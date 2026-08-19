using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public class EstablishmentDetailsBasicDetailsTableMapper :
    IMapper<EstablishmentDetailsModel, GovUkTable>
{
    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        builder.AddRow(new TableCell { Text = "URN", IsBold = true },
                       new TableCell { Text = dto.Urn.Value });

        builder.AddRow(new TableCell { Text = "UKPRN", IsBold = true },
                       new TableCell { Text = dto.Number.Value ?? string.Empty });

        builder.AddRow(new TableCell { Text = "DfE Number", IsBold = true },
                       new TableCell { Text = dto.Number.Value ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Status", IsBold = true },
                       new TableCell { Text = dto.Status.Value ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Address", IsBold = true },
                       new TableCell { Text = dto.Address?.AddressLine1 ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Local authority", IsBold = true },
                       new TableCell { Text = dto.LocalAuthority?.Name ?? string.Empty});

        builder.AddRow(new TableCell { Text = "Part of group", IsBold = true },
                       new TableCell { Text = dto.GroupName });

        builder.AddRow(new TableCell { Text = "Type", IsBold = true },
                       new TableCell { Text = dto.Type.Value });

        builder.AddRow(new TableCell { Text = "Phase of education", IsBold = true },
                       new TableCell { Text = dto.Phase.Value ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Age range", IsBold = true },
                       new TableCell { Text = dto.AgeRange ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Gender", IsBold = true },
                       new TableCell { Text = dto.LifecycleEventOpened?.EventDate.ToShortDateString() ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Religious character", IsBold = true },
                       new TableCell { Text = dto.ReligiousCharacter ?? string.Empty });

        builder.AddRow(new TableCell { Text = "Ofsted", IsBold = true },
                       new TableCell { Text = dto.Ofsted?.InspectionDate is DateOnly inspectionDate
                                              ? $"Latest report {inspectionDate:d MMMM yyyy} (opens in new tab)"
                                              : string.Empty, 
                                              Href = dto.Ofsted?.InspectionOutcome ?? string.Empty,
                                              OpenInNewTab = true});

        return builder.Build();
    }
}
