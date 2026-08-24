using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups;

public sealed class GroupDetailsPageViewModelMapperTests
{
    private static readonly GroupDetailsPageViewModelMapper Mapper = new();

    [Fact]
    public void Map_MapsHeadingAndBasicDetails()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub();

        // Act
        GroupDetailsPageViewModel result = Mapper.Map(readModel);

        // Assert
        Assert.Equal("Test Group", result.Heading);
        Assert.Equal("Details", result.Details.Tab);

        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Collection(
            summary.Rows,
            row => AssertSummaryRow(row, "UID", "123"),
            row => AssertSummaryRow(row, "Group ID", "group-1"),
            row => AssertSummaryRow(row, "UKPRN", "10000001"),
            row => AssertSummaryRow(
                row,
                "Company number",
                "CH123456 (opens in new tab)",
                "https://find-and-update.company-information.service.gov.uk/company/CH123456"),
            row => AssertSummaryRow(row, "Status", "Open"),
            row => AssertSummaryRow(row, "Address", "1 Test Street"),
            row => AssertSummaryRow(row, "Type", "Multi-academy trust"));
    }

    [Fact]
    public void Map_MapsAcademiesTable()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Academies =
            [
                CreateAcademy(
                    urn: "123456",
                    name: "Test Academy")
            ]
        };

        // Act
        GroupDetailsPageViewModel result = Mapper.Map(readModel);

        // Assert
        Assert.Equal("Academies (1)", result.Academies.Tab);

        GovUkTable table =
            Assert.IsType<GovUkTable>(result.Academies.Table);

        Assert.Equal("Academies", table.Caption);

        Assert.Collection(
            table.Columns,
            column =>
            {
                Assert.Equal("Name", column.Text);
                Assert.True(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("URN", column.Text);
                Assert.False(column.IsRowHeader);
            });

        TableRow row = Assert.Single(table.Rows);

        Assert.Collection(
            row.Cells,
            cell =>
            {
                Assert.Equal("Test Academy", cell.Text);
                Assert.Equal("/establishments/123456", cell.Href);
            },
            cell =>
            {
                Assert.Equal("123456", cell.Text);
                Assert.Null(cell.Href);
            });
    }

    [Fact]
    public void Map_MapsGovernanceTables()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                new TrusteeReadModel
                {
                    Id = "T123",
                    FullName = "Test Trustee",
                    StartDate = new DateTime(2020, 1, 15)
                }
            ],
            Members =
            [
                new MemberReadModel
                {
                    Identifier = "M123",
                    FullName = "Test Member",
                    StartDate = new DateTime(2021, 6, 25)
                }
            ]
        };

        // Act
        GroupDetailsPageViewModel result = Mapper.Map(readModel);

        // Assert
        Assert.Equal("Governance", result.Governance.Tab);

        AssertTrusteesTable(result.Governance.TrusteesTable);
        AssertMembersTable(result.Governance.MembersTable);
    }

    [Fact]
    public void Map_CreatesEmptyTables_WhenCollectionsAreEmpty()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub();

        // Act
        GroupDetailsPageViewModel result = Mapper.Map(readModel);

        // Assert
        Assert.Equal("Academies (0)", result.Academies.Tab);

        Assert.Empty(
            Assert.IsType<GovUkTable>(result.Academies.Table).Rows);

        Assert.Empty(
            Assert.IsType<GovUkTable>(
                result.Governance.TrusteesTable).Rows);

        Assert.Empty(
            Assert.IsType<GovUkTable>(
                result.Governance.MembersTable).Rows);
    }

    private static void AssertTrusteesTable(GovUkTable? table)
    {
        GovUkTable trusteesTable = Assert.IsType<GovUkTable>(table);

        Assert.Equal("Trustees", trusteesTable.Caption);
        AssertGovernanceColumns(trusteesTable.Columns);

        TableRow row = Assert.Single(trusteesTable.Rows);

        Assert.Collection(
            row.Cells,
            cell => Assert.Equal("Test Trustee", cell.Text),
            cell => Assert.Equal("T123", cell.Text),
            cell => Assert.Equal("15 January 2020", cell.Text));
    }

    private static void AssertMembersTable(GovUkTable? table)
    {
        GovUkTable membersTable = Assert.IsType<GovUkTable>(table);

        Assert.Equal("Members", membersTable.Caption);
        AssertGovernanceColumns(membersTable.Columns);

        TableRow row = Assert.Single(membersTable.Rows);

        Assert.Collection(
            row.Cells,
            cell => Assert.Equal("Test Member", cell.Text),
            cell => Assert.Equal("M123", cell.Text),
            cell => Assert.Equal("25 June 2021", cell.Text));
    }

    private static void AssertGovernanceColumns(
        IReadOnlyList<TableColumn> columns)
    {
        Assert.Collection(
            columns,
            column =>
            {
                Assert.Equal("Name", column.Text);
                Assert.True(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Governor ID", column.Text);
                Assert.False(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Start date", column.Text);
                Assert.False(column.IsRowHeader);
            });
    }

    private static void AssertSummaryRow(
        SummaryListRow row,
        string expectedKey,
        string expectedText,
        string? expectedHref = null)
    {
        Assert.Equal(expectedKey, row.Key);
        Assert.Equal(expectedText, row.Value.Text);
        Assert.Equal(expectedHref, row.Value.Href);
    }

    private static Academy CreateAcademy(
        string urn,
        string name)
    {
        return new Academy(
            new AcademyId(
                new UniqueReferenceNumber(urn)),
            new AcademyName(name));
    }
}