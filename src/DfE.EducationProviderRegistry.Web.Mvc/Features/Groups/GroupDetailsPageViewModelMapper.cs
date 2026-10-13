using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

internal sealed class GroupDetailsPageViewModelMapper :
    IMapper<GroupReadModel, GroupDetailsPageViewModel>
{
    public GroupDetailsPageViewModel Map(GroupReadModel readModel)
    {
        return new GroupDetailsPageViewModel
        {
            Heading = readModel.Name,
            Details = CreateBasicDetails(readModel),
            Academies = CreateAcademies(readModel),
            Governance = CreateGovernance(readModel)
        };
    }

    private static GroupDetailsTabViewModel CreateBasicDetails(GroupReadModel model)
    {
        List<SummaryListRow> values = [];

        values.AddRange(
            new SummaryListRow(
                "UID",
                new SummaryListValue(text: model.GroupUID.ToString())),
            new SummaryListRow(
                "Group ID",
                new SummaryListValue(text: model.GroupId)));

        if (!string.IsNullOrWhiteSpace(model.UKPRN))
        {
            values.Add(
                new SummaryListRow(
                    "UKPRN",
                    new SummaryListValue(text: model.UKPRN)));
        }

        if (!string.IsNullOrWhiteSpace(model.CompaniesHouseId))
        {
            values.Add(
                new SummaryListRow(
                    "Company number",
                    new SummaryListValue(text: $"{model.CompaniesHouseId} (opens in new tab)", href: $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}")));
        }

        if (!string.IsNullOrWhiteSpace(model.Status))
        {
            values.Add(
                new SummaryListRow(
                    "Status",
                    new SummaryListValue(text: model.Status)));
        }

        if (!string.IsNullOrWhiteSpace(model.Address))
        {
            values.Add(
                new SummaryListRow(
                    "Address",
                    new SummaryListValue(text: model.Address)));
        }

        if (!string.IsNullOrWhiteSpace(model.Type))
        {
            values.Add(
                new SummaryListRow(
                    "Type",
                    new SummaryListValue(model.Type)));
        }

        GovUkSummaryList details = new(rows: values);

        return new()
        {
            Tab = "Details",
            Summary = details
        };
    }

    private static GroupDetailsAcademyTabViewModel CreateAcademies(GroupReadModel model)
    {
        TableColumn[] columns = [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "URN" }
        ];

        List<TableRow> rows = [];

        foreach (Academy academy in model.Academies)
        {
            TableCell name = new()
            {
                Text = academy.Name.ToString(),
                Href = $"/establishments/{academy.Id.Value}"
            };

            TableCell urn = new()
            {
                Text = academy.Id.Value
            };

            rows.Add(new TableRow { Cells = [name, urn] });
        }

        GovUkTable academiesTable = new(
            columns: columns,
            rows: rows,
            caption: "Academies");

        return new()
        {
            Tab = $"Academies ({model.Academies.Count})",
            Table = academiesTable
        };
    }


    private static GroupDetailsGovernanceTabViewModel CreateGovernance(GroupReadModel model)
    {
        return new()
        {
            Tab = "Governance",
            TrusteesTable = CreateTrusteesTable(model.Trustees),
            MembersTable = CreateMembersTable(model.Members)
        };
    }


    private static GovUkTable CreateTrusteesTable(IEnumerable<TrusteeReadModel> trustees)
    {
        TableColumn[] columns = [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "Governor ID" },
            new() { Text = "Start date" }
        ];

        List<TableRow> rows = [];

        foreach (TrusteeReadModel trustee in trustees)
        {
            TableCell name = new()
            {
                Text = trustee.FullName,
            };

            TableCell governorId = new()
            {
                Text = trustee.Id
            };

            TableCell startDate = new()
            {
                Text = trustee.StartDate.ToString("dd MMMM yyyy")
            };

            rows.Add(new TableRow { Cells = [name, governorId, startDate] });
        }

        return new(columns, rows, caption: "Trustees");
    }

    private static GovUkTable CreateMembersTable(IEnumerable<MemberReadModel> members)
    {
        TableColumn[] columns = [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "Governor ID" },
            new() { Text = "Start date" }
        ];

        List<TableRow> rows = [];

        foreach (MemberReadModel member in members)
        {
            TableCell name = new()
            {
                Text = member.FullName,
            };

            TableCell governorId = new()
            {
                Text = member.Identifier
            };

            TableCell startDate = new()
            {
                Text = member.StartDate.ToString("dd MMMM yyyy")
            };

            rows.Add(new TableRow { Cells = [name, governorId, startDate] });
        }

        return new(columns, rows, caption: "Members");
    }
}
