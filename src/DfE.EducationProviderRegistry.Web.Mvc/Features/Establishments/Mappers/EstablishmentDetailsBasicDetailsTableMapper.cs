using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public sealed class EstablishmentDetailsBasicDetailsTableMapper : IMapper<EstablishmentDetailsModel, GovUkTable>
{
    private const string OfstedProviderBase = "http//www.ofsted.gov.uk/oxedu_providers/full/(urn)/";
    private static readonly CultureInfo UkCulture = CultureInfo.GetCultureInfo("en-GB");

    public GovUkTable Map(EstablishmentDetailsModel dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        EstablishmentStatus? status = dto.Status?.Status;
        string? type = dto.Type?.Value;

        string? statusText = MapStatus(status, dto.OpenedDate, dto.ClosedDate);
        AddTextRow("Status", MapStatus(
            status: status,
            openedDate: dto.OpenedDate,
            closedDate: dto.ClosedDate));

        AddRow("ID numbers", new TableCell
        {
            Rows = MapIdentifiers(dto.Identifiers)
        });

        AddTextRow("Headteacher", dto.Headteacher);
        AddTextRow("Type", type);
        AddTextRow("Phase of education", dto.Phase?.Value);
        AddTextRow("Address", MappingHelpers.CombineAddress(dto.Address, dto.Name?.Value));
        AddTextRow("Local authority", dto.LocalAuthority?.Name);

        EstablishmentGroupModel? group = dto.Group;
        if (group is not null)
        {
            AddTextRow("Part of group", MapGroupName(group.name), href: $"/groups/{group.uid}");
        }

        AddTextRow("Gender", dto.Gender);
        AddTextRow("Number of pupils", MapPupilCounts(dto.Pupils));
        AddTextRow("Pupils eligible for free school meals", MapFreeSchoolMeals(dto.FreeSchoolMeals));

        AddTextRow("Age range", dto.AgeRange);
        AddTextRow("Religious character", MapReligiousText(dto.ReligiousCharacter));
        AddTextRow("Religious ethos", MapReligiousText(dto.ReligiousEthos));
        AddTextRow("Diocese", MapDioceseText(dto.Diocese));
        AddTextRow("Type of SEN provision", dto.SenProvision);

        string? website = dto.ContactDetails?.Website;
        AddTextRow("Website", website, href: MappingHelpers.CreateWebsiteUrl(website), openInNewTab: true);
        AddTextRow("Telephone number", dto.ContactDetails?.TelephoneNumber);

        string? ofstedUrl = MapOfstedUrl(status, type, dto.Identifiers.Urn);
        if (ofstedUrl is not null)
        {
            AddTextRow("Ofsted", "View Ofsted report", href: ofstedUrl, openInNewTab: true);
        }


        return builder.Build();

        void AddRow(string lable, TableCell value)
        {
            builder.AddRow(
                new TableCell { Text = lable, IsBold = true }, value);
        }

        void AddTextRow(string label, string? value, string? href = null, bool openInNewTab = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AddRow(label, new TableCell { Text = value.Trim(), Href = href, OpenInNewTab = openInNewTab });
        }
    }

    private static string? MapStatus(EstablishmentStatus? status, DateOnly? openedDate, DateOnly? closedDate)
    {
        (string label, DateOnly? date) = status switch
        {
            EstablishmentStatus.Open => ("Open", openedDate),
            EstablishmentStatus.ProposedToOpen => ("Proposed to open", openedDate),
            EstablishmentStatus.Closed => ("Closed", closedDate),
            EstablishmentStatus.OpenButProposedToClose => ("Open but proposed to close", closedDate),
            _ => (string.Empty, null)
        };

        if (!date.HasValue)
            return null;

        string formattedDate = date.Value.ToString("dd MMMM yyyy", UkCulture);
        return $"{label} on {formattedDate}";
    }

    private static IReadOnlyList<TableCellRow> MapIdentifiers(EstablishmentIdentifiersModel identifiers)
    {
        (string Label, string? Value)[] values =
        [
            ("LAESTAB", identifiers.LaEstab),
            ("UKPRN", identifiers.Ukprn),
            ("URN", identifiers.Urn)
        ];

        List<TableCellRow> rows = [];
        foreach ((string? label, string? value) in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                rows.Add(new TableCellRow
                {
                    Label = new TableCell { Text = label },
                    Value = new TableCell { Text = value }
                });
            }
        }

        return rows;
    }

    private static string? MapGroupName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLowerInvariant());
    }

    private static string? MapPupilCounts(EstablishmentPupils? pupilCounts)
    {
        if (!pupilCounts.HasValue)
            return null;

        EstablishmentPupils counts = pupilCounts.Value;
        return $"{counts.Total} ({counts.Boys} boys, {counts.Girls} girls)";
    }

    private static string? MapFreeSchoolMeals(EstablishmentFreeSchoolMeals? freeSchoolMeals)
    {
        if (freeSchoolMeals is null)
            return null;

        return $"{freeSchoolMeals.Number} ({freeSchoolMeals.Percentage}%)";
    }

    private static string? MapReligiousText(string? value) =>
        ExcludeText(value, "None", "Does not apply");

    private static string? MapDioceseText(string? value) =>
       ExcludeText(value, "Not applicable");

    private static string? MapOfstedUrl(EstablishmentStatus? status, string? type, string urn)
    {
        bool hasEligibleStatus = status is EstablishmentStatus.Open
            or EstablishmentStatus.Closed
            or EstablishmentStatus.OpenButProposedToClose;

        if (!hasEligibleStatus)
            return null;

        type = type?.Trim();
        if (string.IsNullOrWhiteSpace(type)
            || string.Equals(type, "Other independent school", StringComparison.OrdinalIgnoreCase)
            || string.Equals(type, "Other independent special school", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return OfstedProviderBase + Uri.EscapeDataString(urn);
    }

    private static string? ExcludeText(string? value, params string[] excludedValues)
    {
        string? text = value?.Trim();
        if (excludedValues.Contains(text, StringComparer.OrdinalIgnoreCase))
            return null;

        return text;
    }
}
