using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;

internal sealed class GroupDetailsPageViewModelMapper : IMapper<GroupReadModel, GroupDetailsPageViewModel>
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
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithColumns(
                new TableColumn { IsRowHeader = true },
                new TableColumn());

        builder.AddRow(
            new TableCell() { Text = "ID Numbers" },
            new TableCell() { Rows = MapIdentifiers(model) });

        AddTextRow("Status", model.Status);
        AddTextRow("Address", model.Address);
        AddTextRow("Education group type", model.Type);

        return new()
        {
            Tab = "Details",
            Table = builder.Build()
        };

        void AddTextRow(string label, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                builder.AddRow(
                    new TableCell { Text = label },
                    new TableCell { Text = value.Trim() });
            }
        }
    }

    private static IReadOnlyList<TableCellRow> MapIdentifiers(GroupReadModel model)
    {
        List<TableCellRow> rows = [];

        AddIdentifier(model.GroupId, "Group Id");
        AddIdentifier(model.GroupUID.ToString(), "UID");
        AddIdentifier(model.UKPRN, "UKPRN");
        AddIdentifier(
            identifier: model.CompaniesHouseId,
            name: "Company number",
            href: $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}");

        return rows;

        void AddIdentifier(string? identifier, string name, string? href = null)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return;

            rows.Add(new TableCellRow()
            {
                Label = new TableCell()
                {
                    Text = href is null ? identifier : $"{identifier}",
                    Href = href,
                    OpenInNewTab = true
                },
                Value = new TableCell() { Text = name }
            });
        }
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
