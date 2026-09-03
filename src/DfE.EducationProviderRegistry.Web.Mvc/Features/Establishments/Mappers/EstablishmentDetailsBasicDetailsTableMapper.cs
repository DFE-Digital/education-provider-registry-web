using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using System.Globalization;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public class EstablishmentDetailsBasicDetailsTableMapper :
    IMapper<EstablishmentDetailsModel, GovUkTable>
{
    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        const string DataUnavailable = "Yet to be provisioned";

        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        builder.AddRow(
            new TableCell { Text = "Status", IsBold = true },
            new TableCell { Text = dto.Status?.Value ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "ID numbers", IsBold = true },
            new TableCell
            {
                Rows =
                [
                    new TableCellRow
                    {
                        Label = new TableCell { Text = "LAESTAB" },
                        Value = new TableCell { Text = dto.Number?.Value ?? string.Empty }
                    },
                    new TableCellRow
                    {
                        Label = new TableCell { Text = "UKPRN" },
                        Value = new TableCell { Text = dto.Number?.Value ?? string.Empty }
                    },
                    new TableCellRow
                    {
                        Label = new TableCell { Text = "URN" },
                        Value = new TableCell { Text = dto.Urn.Value }
                    }
                ]
            });

        builder.AddRow(
            new TableCell { Text = "Headteacher", IsBold = true },
            new TableCell { Text = dto.Headteacher ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Type", IsBold = true },
            new TableCell { Text = dto.Type?.Value });

        builder.AddRow(
            new TableCell { Text = "Phase of education", IsBold = true },
            new TableCell { Text = dto.Phase?.Value ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Address", IsBold = true },
            new TableCell { Text = MappingHelpers.CombineAddress(dto.Address, dto.Name?.Value)});

        builder.AddRow(
            new TableCell { Text = "Local authority", IsBold = true },
            new TableCell { Text = dto.LocalAuthority?.Name ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Part of", IsBold = true },
            new TableCell 
            { 
                Text = dto.Group?.GroupName is null ? string.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dto.Group?.GroupName.ToLower()!) ?? string.Empty,
                Href = MappingHelpers.CreateLinkUrl("/groups/", dto.Group?.Code)
            });

        builder.AddRow(
            new TableCell { Text = "Age range", IsBold = true },
            new TableCell { Text = dto.AgeRange ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Gender", IsBold = true },
            new TableCell { Text = DataUnavailable });

        builder.AddRow(
            new TableCell { Text = "Number of pupils", IsBold = true },
            new TableCell { Text = DataUnavailable });

        builder.AddRow(
            new TableCell { Text = "Pupils capacity", IsBold = true },
            new TableCell { Text = dto.AgeRange ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Religious character", IsBold = true },
            new TableCell { Text = dto.ReligiousCharacter ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Type of SEN provision", IsBold = true },
            new TableCell { Text = dto.SenProvision ?? "Not recorded" });

        if (dto.ContactDetails?.Website is not null)
        {
            builder.AddRow(
                new TableCell { Text = "Website", IsBold = true },
                new TableCell { Text = dto.ContactDetails.Website, Href = dto.ContactDetails.Website, OpenInNewTab = true });
        }

        if(dto.ContactDetails?.TelephoneNumber is not null)
        {
            builder.AddRow(
                new TableCell { Text = "Telephone number", IsBold = true },
                new TableCell { Text = dto.ContactDetails?.TelephoneNumber ?? string.Empty });
        }

        builder.AddRow(
            new TableCell { Text = "Ofsted", IsBold = true },
            new TableCell
            {
                Text = dto.Ofsted?.InspectionDate is DateOnly inspectionDate
                    ? $"Latest report {inspectionDate:d MMMM yyyy} (opens in new tab)"
                    : string.Empty,
                Href = dto.Ofsted?.InspectionOutcome ?? string.Empty,
                OpenInNewTab = true
            });

        builder.AddRow(
            new TableCell { Text = "School profiles service", IsBold = true },
            new TableCell { Text = DataUnavailable });

        return builder.Build();
    }
}