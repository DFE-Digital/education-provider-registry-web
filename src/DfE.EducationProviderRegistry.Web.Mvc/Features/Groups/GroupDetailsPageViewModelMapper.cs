using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
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
        GovUkTableColumn[] columns = [
            new("UID") { IsRowHeader = true},
            new("Group ID"),
            new("UKPRN"),
            new("Company number"),
            new("Status"),
            new("Address"),
            new("Type")];

        GovUkTable detailsTable = new(
            columns: columns,
            rows: [
                    [new GovUkTableCell() { Text = model.GroupUID.ToString() } ],
                    [new GovUkTableCell() { Text = model.GroupId } ],
                    [new GovUkTableCell() { Text = model.UKPRN } ],
                    [new GovUkTableCell()
                        {
                            Text = string.IsNullOrWhiteSpace(model.CompaniesHouseId) ? string.Empty : $"{model.CompaniesHouseId} (opens in new tab)", // TODO 
                            Href =  $"https://find-and-update.company-information.service.gov.uk/company/{model.CompaniesHouseId}"
                        }
                    ],
                    [new GovUkTableCell() { Text = model.Status } ],
                    [new GovUkTableCell() { Text = model.Address  } ],
                    [new GovUkTableCell() { Text = model.Type } ]
                ],
            caption: "Details");

        return new()
        {
            Tab = "Details",
            Details = detailsTable
        };
    }

    private static GroupDetailsAcademyTabViewModel CreateAcademies(GroupReadModel model)
    {
        GovUkTableColumn[] columns = [
            new("Name") { IsRowHeader = true},
            new("URN")
        ];

        List<GovUkTableCell[]> rows = new();

        foreach (Academy academy in model.Academies)
        {
            GovUkTableCell name = new()
            {
                Text = academy.Name.ToString(),
                Href = $"/establishment/{academy.Id.Value}"
            };

            GovUkTableCell urn = new()
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
            Academies = academiesTable
        };
    }


    private static GroupDetailsGovernanceTabViewModel CreateGovernance(GroupReadModel model)
    {
        return new()
        {
            Tab = "Governance",
            Trustees = CreateTrusteesTable(model.Trustees),
            Members = CreateMembersTable(model.Members)
        };
    }


    private static GovUkTable CreateTrusteesTable(IEnumerable<TrusteeReadModel> trustees)
    {
        GovUkTableColumn[] columns = [
            new("Name") { IsRowHeader = true},
            new("Governor ID"),
            new("Start date")
        ];

        List<GovUkTableCell[]> rows = [];

        foreach (TrusteeReadModel trustee in trustees)
        {
            GovUkTableCell name = new()
            {
                Text = trustee.FullName,
            };

            GovUkTableCell governorId = new()
            {
                Text = trustee.Id
            };

            GovUkTableCell startDate = new()
            {
                Text = trustee.StartDate.ToString("dd MMMM yyyy")
            };

            rows.Add([name, governorId, startDate]);
        }

        return new(columns, rows, caption: "Trustees");
    }

    private static GovUkTable CreateMembersTable(IEnumerable<MemberReadModel> members)
    {
        GovUkTableColumn[] columns = [
            new("Name") { IsRowHeader = true},
            new("Governor ID"),
            new("Start date")
        ];

        List<GovUkTableCell[]> rows = [];

        foreach (MemberReadModel member in members)
        {
            GovUkTableCell name = new()
            {
                Text = member.FullName,
            };

            GovUkTableCell governorId = new()
            {
                Text = member.Identifier
            };

            GovUkTableCell startDate = new()
            {
                Text = member.StartDate.ToString("dd MMMM yyyy")
            };

            rows.Add([name, governorId, startDate]);
        }

        return new(columns, rows, caption: "Trustees");
    }
}
