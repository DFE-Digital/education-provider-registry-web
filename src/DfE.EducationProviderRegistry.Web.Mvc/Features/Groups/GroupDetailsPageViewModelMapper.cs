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
        // TODO convert to GDSSummaryList?
        GovUkSummaryList details = new(
        [
            new(
                "UID",
                new SummaryListValue
                {
                    Text = model.GroupUID.ToString()
                }),

            new(
                "Group ID",
                new SummaryListValue
                {
                    Text = model.GroupId
                }),

            new(
                "UKPRN",
                new SummaryListValue
                {
                    Text = model.UKPRN
                }),

            new(
                "Company number",
                new SummaryListValue
                {
                    Text = $"{model.CompaniesHouseId} (opens in new tab)",
                    Href = $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}"
                }),

            new(
                "Status",
                new SummaryListValue
                {
                    Text = model.Status
                }),

            new(
                "Address",
                new SummaryListValue
                {
                    Text = model.Address
                }),
            new(
                "Type",
                new SummaryListValue
                {
                    Text = model.Type
                })
        ]);

        return new()
        {
            Tab = "Details",
            Summary = details
        };
    }

    private static GroupDetailsAcademyTabViewModel CreateAcademies(GroupReadModel model)
    {
        TableColumn[] columns = [
            new("Name") { IsRowHeader = true},
            new("URN")
        ];

        List<TableCell[]> rows = new();

        foreach (Academy academy in model.Academies)
        {
            TableCell name = new()
            {
                Text = academy.Name.ToString(),
                Href = $"/establishment/{academy.Id.Value}"
            };

            TableCell urn = new()
            {
                Text = academy.Id.Value
            };

            rows.Add([urn, name]);
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
            new("Name") { IsRowHeader = true},
            new("Governor ID"),
            new("Start date")
        ];

        List<TableCell[]> rows = [];

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

            rows.Add([name, governorId, startDate]);
        }

        return new(columns, rows, caption: "Trustees");
    }

    private static GovUkTable CreateMembersTable(IEnumerable<MemberReadModel> members)
    {
        TableColumn[] columns = [
            new("Name") { IsRowHeader = true},
            new("Governor ID"),
            new("Start date")
        ];

        List<TableCell[]> rows = [];

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

            rows.Add([name, governorId, startDate]);
        }

        return new(columns, rows, caption: "Members");
    }
}
