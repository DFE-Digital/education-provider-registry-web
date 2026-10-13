using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public sealed class EstablishmentDetailsBasicDetailsTableMapper : IMapper<EstablishmentDetailsModel, GovUkTable>
{
    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        builder.AddRow(
            new TableCell { Text = "Status", IsBold = true },
            new TableCell { Text = MapStatus(dto.Status) });


        List<TableCellRow> identifierValueRows = [];

        if (!string.IsNullOrWhiteSpace(dto.Identifiers.LaEstab))
        {
            identifierValueRows.Add(
                new TableCellRow
                {
                    Label = new TableCell { Text = "LAESTAB" },
                    Value = new TableCell { Text = dto.Identifiers.LaEstab }
                });
        }

        if (!string.IsNullOrWhiteSpace(dto.Identifiers.Ukprn))
        {
            identifierValueRows.Add(
                new TableCellRow
                {
                    Label = new TableCell { Text = "UKPRN" },
                    Value = new TableCell { Text = dto.Identifiers.Ukprn }
                });
        }

        if (!string.IsNullOrWhiteSpace(dto.Identifiers.Urn))
        {
            identifierValueRows.Add(
                new TableCellRow
                {
                    Label = new TableCell { Text = "URN" },
                    Value = new TableCell { Text = dto.Identifiers.Urn }
                });
        }

        if (identifierValueRows.Count > 0)
        {
            builder.AddRow(
                new TableCell { Text = "ID numbers", IsBold = true },
                new TableCell
                {
                    Rows = identifierValueRows
                });
        }

        if (!string.IsNullOrWhiteSpace(dto.Headteacher))
        {
            builder.AddRow(
                new TableCell { Text = "Headteacher", IsBold = true },
                new TableCell { Text = dto.Headteacher });
        }

        builder.AddRow(
            new TableCell { Text = "Type", IsBold = true },
            new TableCell { Text = dto.Type?.Value });

        builder.AddRow(
            new TableCell { Text = "Phase of education", IsBold = true },
            new TableCell { Text = dto.Phase?.Value });

        builder.AddRow(
            new TableCell { Text = "Address", IsBold = true },
            new TableCell { Text = MappingHelpers.CombineAddress(dto.Address, dto.Name?.Value) });

        if (dto.LocalAuthority is not null)
        {
            builder.AddRow(
            new TableCell { Text = "Local authority", IsBold = true },
            new TableCell { Text = dto.LocalAuthority.Name });
        }

        if (dto.Group is not null)
        {
            builder.AddRow(
            new TableCell { Text = "Part of group", IsBold = true },
            new TableCell
            {
                Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dto.Group.name.ToLowerInvariant()),
                Href = MappingHelpers.CreateLinkUrl("/groups/", dto.Group.code)
            });
        }

        builder.AddRow(
            new TableCell { Text = "Age range", IsBold = true },
            new TableCell { Text = dto.AgeRange ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Gender", IsBold = true },
            new TableCell { Text = dto.Gender ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Number of pupils", IsBold = true },
            new TableCell { Text = dto.Pupils?.Total.ToString() ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Pupils capacity", IsBold = true },
            new TableCell { Text = dto.AgeRange ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Religious character", IsBold = true },
            new TableCell { Text = dto.ReligiousCharacter ?? string.Empty });

        builder.AddRow(
            new TableCell { Text = "Type of SEN provision", IsBold = true },
            new TableCell { Text = dto.SenProvision ?? "Not recorded" });

        if (dto.ContactDetails is not null &&
            !string.IsNullOrWhiteSpace(dto.ContactDetails.Website))
        {
            builder.AddRow(
                new TableCell { Text = "Website", IsBold = true },
                new TableCell { Text = dto.ContactDetails.Website, Href = MappingHelpers.CreateWebsiteUrl(dto.ContactDetails.Website), OpenInNewTab = true });
        }

        if (dto.ContactDetails is not null &&
            !string.IsNullOrWhiteSpace(dto.ContactDetails.TelephoneNumber))
        {
            builder.AddRow(
                new TableCell { Text = "Telephone number", IsBold = true },
                new TableCell { Text = dto.ContactDetails.TelephoneNumber ?? string.Empty });
        }

        builder.AddRow(
            new TableCell { Text = "Ofsted", IsBold = true },
            new TableCell
            {
                Text = dto.Ofsted?.InspectionDate is DateOnly inspectionDate
                    ? $"Latest report {inspectionDate:d MMMM yyyy} (opens in new tab)"
                    : string.Empty,
                Href = dto.Ofsted?.ReportUrl ?? string.Empty,
                OpenInNewTab = true
            });

        return builder.Build();
    }

    private static string MapStatus(EstablishmentStatusModel? status)
    {
        if (status is null)
        {
            return string.Empty;
        }

        return status.Status switch
        {
            EstablishmentStatus.Open => "Open",
            EstablishmentStatus.Closed => "Closed",
            EstablishmentStatus.OpenButProposedToClose => "Open but proposed to close",
            EstablishmentStatus.ProposedToOpen => "Proposed to open",
            _ => string.Empty
        };
    }
}
