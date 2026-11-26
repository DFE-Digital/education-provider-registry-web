using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

internal sealed class GroupDetailsPageViewModelMapper : IMapper<GroupReadModel, GroupDetailsPageViewModel>
{
    private static readonly CultureInfo UkCulture = CultureInfo.GetCultureInfo("en-GB");
    private readonly TimeProvider _timeProvider;

    public GroupDetailsPageViewModelMapper(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public GroupDetailsPageViewModel Map(GroupReadModel readModel)
    {
        return new GroupDetailsPageViewModel
        {
            Heading = readModel.Name,
            Details = CreateBasicDetails(readModel),
            Establishments = CreateAcademies(readModel),
            Governance = CreateGovernance(readModel)
        };
    }

    private static GroupDetailsTabViewModel CreateBasicDetails(GroupReadModel model)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        builder.AddRow(
            new TableCell() { Text = "ID Numbers" },
            new TableCell() { Rows = MapIdentifiers(model) });

        AddRow(builder, "Status", MapStatus(model.StatusLabel, model.OpenedDate, model.ClosedDate));
        AddRow(builder, "Address", model.Address);
        AddRow(builder, "Education group type", model.TypeName);

        return new()
        {
            Tab = "Details",
            Table = builder.Build()
        };
    }

    private static void AddRow(GovUkTableBuilder builder, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        builder.AddRow(
            new TableCell { Text = label },
            new TableCell { Text = value.Trim() });
    }

    private static List<TableCellRow> MapIdentifiers(GroupReadModel model)
    {
        (string Label, string? value, string? Href)[] values =
        [
            ("Group Id", model.GroupId, null),
            ("UID", model.GroupUID.ToString(), null),
            ("UKPRN" , model.UKPRN, null),
            ("Company number", model.CompaniesHouseId, $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}")
        ];

        List<TableCellRow> rows = [];

        foreach ((string identifier, string? label, string? href) in values)
        {
            if (!string.IsNullOrWhiteSpace(label))
            {
                rows.Add(new TableCellRow
                {
                    Label = new TableCell { Text = label, Href = href, OpenInNewTab = true },
                    Value = new TableCell { Text = identifier }
                });
            }
        }

        return rows;
    }

    private static string? MapStatus(string? status, DateOnly? openedDate, DateOnly? closedDate)
    {
        (string label, DateOnly? date) = status switch
        {
            "Open" => ("Open", openedDate),
            "Proposed to open" => ("Proposed to open", openedDate),
            "Closed" => ("Closed", closedDate),
            "Open but proposed to close" => ("Open but proposed to close", closedDate),
            _ => (string.Empty, null)
        };

        if (!date.HasValue)
            return null;

        string formattedDate = date.Value.ToString("dd MMMM yyyy", UkCulture);
        return $"{label} on {formattedDate}";
    }


    private static GroupDetailsEstablishmentTabViewModel CreateAcademies(GroupReadModel model)
    {

        TableColumn[] columns =
        [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "URN" }
        ];

        string establishmentLabelForGroupType = model.TypeCode switch
        {
            GroupTypeCodes.Federation => "Providers",
            GroupTypeCodes.ChildrensCentresGroup or GroupTypeCodes.ChildrensCentresCollaboration => "Centres",
            _ => "Academies"
        };

        string tabLabel = $"{establishmentLabelForGroupType} ({model.Establishments.Count})";

        if (model.Establishments.Count == 0)
        {
            return new()
            {
                Tab = tabLabel,
                Table = null,
                NoEstablishmentsMessage = "No centres are currently recorded for this group."
            };
        }

        List<TableRow> rows = [];

        foreach (EstablishmentReadModel establishment in model.Establishments)
        {
            TableCell name = new()
            {
                Text = establishment.Name,
                Href = EstablishmentRoutes.Details(urn: establishment.Urn).ToString()
            };

            TableCell urn = new()
            {
                Text = establishment.Urn
            };

            rows.Add(
                new TableRow
                {
                    Cells = [name, urn]
                });
        }

        return new()
        {
            Tab = tabLabel,
            Table = new GovUkTable(
                columns: columns,
                rows: rows,
                caption: establishmentLabelForGroupType)
        };
    }


    private GroupDetailsGovernanceTabViewModel CreateGovernance(GroupReadModel model)
    {

        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

        DateOnly twelveMonthsAgo = today.AddYears(-1);

        IReadOnlyCollection<TrusteeReadModel> activeTrustees = model.Trustees
            .Where(trustee =>
                trustee.StartDate <= today &&
                    (!trustee.EndDate.HasValue || trustee.EndDate.Value >= today))
            .ToArray();

        IReadOnlyCollection<TrusteeReadModel> historicTrustees = model.Trustees
            .Where(trustee =>
                trustee.EndDate.HasValue &&
                    trustee.EndDate.Value < today &&
                        trustee.EndDate.Value >= twelveMonthsAgo)
            .ToArray();

        return new()
        {
            Tab = "Governance",
            TrusteesTable = CreateTrusteesTable(activeTrustees, caption: "Trustees"),
            HistoricTrusteesTable = CreateTrusteesTable(historicTrustees, caption: "Trustees who have left within the last 12 months"),
            MembersTable = CreateMembersTable(model.Members)
        };
    }


    private static GovUkTable CreateTrusteesTable(
        IEnumerable<TrusteeReadModel> trustees,
        string caption)
    {
        TableColumn[] columns =
        [
            new() { Text = "Name and role", IsRowHeader = true },
            new() { Text = "Governor ID" },
            new() { Text = "Appointed by" },
            new() { Text = "Term" }
        ];

        List<TableRow> rows = [];

        foreach (TrusteeReadModel trustee in trustees)
        {
            string name = FormatName(trustee.Title, trustee.FullName);

            rows.Add(
                new TableRow
                {
                    Cells =
                    [
                        new()
                    {
                        Text = string.IsNullOrWhiteSpace(trustee.Role)
                            ? name
                            : $"{name} ({trustee.Role})"
                    },
                    new()
                    {
                        Text = trustee.Id
                    },
                    new()
                    {
                        Text = trustee.AppointedBy
                    },
                    new()
                    {
                        Text = FormatTerm(trustee.StartDate, trustee.EndDate)
                    }
                    ]
                });
        }

        return new(columns, rows, caption: caption);
    }

    private static GovUkTable CreateMembersTable(
        IEnumerable<MemberReadModel> members)
    {
        TableColumn[] columns =
        [
            new() { Text = "Name and role", IsRowHeader = true },
            new() { Text = "Governor ID" },
            new() { Text = "Appointed by" },
            new() { Text = "Term" }
        ];

        List<TableRow> rows = [];

        foreach (MemberReadModel member in members)
        {
            rows.Add(
                new TableRow
                {
                    Cells =
                    [
                        new()
                    {
                        Text = FormatName(member.Title, member.FullName)
                    },
                    new()
                    {
                        Text = member.Identifier
                    },
                    new()
                    {
                        Text = member.AppointedBy
                    },
                    new()
                    {
                        Text = FormatTerm(member.StartDate, member.EndDate)
                    }
                    ]
                });
        }

        return new(columns, rows, caption: "Members");
    }

    private static string FormatName(string? title, string fullName)
    {
        return string.IsNullOrWhiteSpace(title)
            ? fullName
            : $"{title} {fullName}";
    }

    private static string FormatTerm(DateOnly startDate, DateOnly? endDate)
    {
        string formattedStartDate = startDate.ToString("d MMMM yyyy");

        return endDate.HasValue
            ? $"{formattedStartDate} to {endDate.Value.ToString("d MMMM yyyy")}"
            : $"From {formattedStartDate}";
    }
}
