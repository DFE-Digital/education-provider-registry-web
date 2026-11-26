using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Microsoft.Extensions.Time.Testing;
using GroupReadModelTestDoubles = DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails.GroupReadModelTestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups;

public sealed class GroupDetailsPageViewModelMapperTests
{
    private readonly FakeTimeProvider _timeProvider;
    private readonly GroupDetailsPageViewModelMapper _mapper;

    public GroupDetailsPageViewModelMapperTests()
    {
        _timeProvider = new(
            new DateTimeOffset(
                2026,
                9,
                3,
                0,
                0,
                0,
                TimeSpan.Zero));

        _mapper = new(_timeProvider);
    }

    [Fact]
    public void Map_Maps_Heading_And_GroupIdentifiers()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithName("Test Group")
              .WithGroupId("group-1")
              .WithGroupUid(123L)
              .WithCompaniesHouseId("123456789")
              .WithUkprn("12345678")
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        Assert.Equal("Test Group", result.Heading);
        Assert.Equal("Details", result.Details.Tab);

        Assert.Collection(
            GetDetailsValue(result, "ID Numbers").Rows,
            row => Assert.Equal(("group-1", "Group Id"), (row.Label.Text, row.Value.Text)),
            row => Assert.Equal(("123", "UID"), (row.Label.Text, row.Value.Text)),
            row => Assert.Equal(("12345678", "UKPRN"), (row.Label.Text, row.Value.Text)),
            row => Assert.Equal(("123456789", "Company number"), (row.Label.Text, row.Value.Text)));
    }

    [Theory]
    [InlineData(" \r\n  \n ")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void Map_DoesNotMap_NullOrWhitespace_UKPRN(string? ukprn)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithUkprn(ukprn)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        Assert.Null(
            GetDetailsValue(result, "ID Numbers").Rows
                .SingleOrDefault(row => row.Value.Text == "UKPRN"));
    }

    [Fact]
    public void Map_Maps_Ukprn()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithUkprn("testUkprn")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCellRow identifer = Assert.Single(
            GetDetailsValue(result, "ID Numbers").Rows,
            row => row.Value.Text == "UKPRN");
        Assert.Equal("testUkprn", identifer.Label.Text);
        Assert.Null(identifer.Label.Href);
    }

    [Theory]
    [InlineData(" \r\n  \n ")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void Map_DoesNotMap_NullOrWhitespace_CompaniesHouseId(string? companiesHouseId)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithCompaniesHouseId(companiesHouseId)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        Assert.Null(
            GetDetailsValue(result, "ID Numbers").Rows
                .SingleOrDefault(row => row.Value.Text == "Company number"));
    }

    [Fact]
    public void Map_Maps_CompaniesHouseId()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithCompaniesHouseId("testCompaniesHouseId")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCellRow identifer = Assert.Single(
           GetDetailsValue(result, "ID Numbers").Rows,
           row => row.Value.Text == "Company number");
        Assert.Equal("testCompaniesHouseId", identifer.Label.Text);
        Assert.Equal("https://find-and-update.company-information.service.gov.uk/company/testCompaniesHouseId", identifer.Label.Href);
        Assert.True(identifer.Label.OpenInNewTab);
        Assert.Null(identifer.Value.Href);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" \r\n  \n ")]
    [InlineData(" ")]
    [InlineData("")]
    public void Map_DoesNotMap_NullOrWhitespace_Status(string? status)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithStatus(status)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkTable table = Assert.IsType<GovUkTable>(result.Details.Table);
        Assert.Null(table.Rows.SingleOrDefault((row) => row.Cells[0].Text == "Status"));
    }

    [Theory]
    [InlineData("Open", 2024, 1, 1, "Open on 01 January 2024")]
    [InlineData("Proposed to open", 2024, 1, 1, "Proposed to open on 01 January 2024")]
    public void Map_Maps_Open_Statuses(
    string status,
    int year,
    int month,
    int day,
    string expected)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithStatus(status)
                .WithOpenedDate(new DateOnly(year, month, day))
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCell value = GetDetailsValue(result, "Status");
        Assert.Equal(expected, value.Text);
        Assert.Null(value.Href);
    }

    [Theory]
    [InlineData("Closed", "Closed on 01 January 2024")]
    [InlineData("Open but proposed to close", "Open but proposed to close on 01 January 2024")]
    public void Map_Maps_Closed_Statuses(
    string status,
    string expected)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithStatus(status)
                .WithClosedDate(new DateOnly(2024, 1, 1))
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCell value = GetDetailsValue(result, "Status");
        Assert.Equal(expected, value.Text);
        Assert.Null(value.Href);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" \r\n  \n ")]
    [InlineData(" ")]
    [InlineData("")]
    public void Map_DoesNotMap_NullOrWhitespace_Address(string? address)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithAddress(address)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkTable table = Assert.IsType<GovUkTable>(result.Details.Table);
        Assert.Null(table.Rows.SingleOrDefault((row) => row.Cells[0].Text == "Address"));
    }

    [Fact]
    public void Map_Maps_Address()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithAddress("testAddress, testVille, TE5 1ST")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCell value = GetDetailsValue(result, "Address");
        Assert.Equal("testAddress, testVille, TE5 1ST", value.Text);
        Assert.Null(value.Href);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" \r\n  \n ")]
    [InlineData(" ")]
    [InlineData("")]
    public void Map_DoesNotMap_NullOrWhitespace_GroupType(string? type)
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
              .WithTypeName(type)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkTable table = Assert.IsType<GovUkTable>(result.Details.Table);
        Assert.Null(table.Rows.SingleOrDefault((row) => row.Cells[0].Text == "Education group type"));
    }

    [Fact]
    public void Map_Maps_GroupType()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithTypeName("Single-Academy trust")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        TableCell value = GetDetailsValue(result, "Education group type");
        Assert.Equal("Single-Academy trust", value.Text);
        Assert.Null(value.Href);
    }

    [Fact]
    public void Map_MapsAcademiesTable()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Establishments =
            [
                new EstablishmentReadModel()
                {
                    Name = "Test Academy",
                    Urn = "123456"
                }
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        Assert.Equal("Academies (1)", result.Establishments.Tab);

        GovUkTable table =
            Assert.IsType<GovUkTable>(result.Establishments.Table);

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
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Test Trustee",
                role: "Chair of Trustees",
                startDate: new DateOnly(2020, 1, 15),
                endDate: new DateOnly(2030, 1, 15),
                title: "Mr",
                appointedBy: "Appointed by academy members")
            ],
            Members =
            [
                MemberReadModelTestDoubles.Stub(
                identifier: "M123",
                fullName: "Test Member",
                startDate: new DateOnly(2021, 6, 25),
                endDate: null,
                title: "Mrs",
                appointedBy: "Appointed by academy members")
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        Assert.Equal("Governance", result.Governance.Tab);

        AssertTrusteesTable(result.Governance.TrusteesTable);
        AssertMembersTable(result.Governance.MembersTable);
    }

    [Fact]
    public void Map_DoesNotPrefixName_WhenTitleIsNull()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Test Trustee",
                role: "Trustee",
                startDate: new DateOnly(2020, 1, 15),
                endDate: null,
                title: null)
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable trusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.TrusteesTable);

        TableRow row = Assert.Single(trusteesTable.Rows);

        Assert.Equal(
            "Test Trustee (Trustee)",
            row.Cells[0].Text);
    }

    [Fact]
    public void Map_MapsTrusteesWhoLeftWithinLastTwelveMonthsToHistoricTable()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Historic Trustee",
                role: "Chair of Trustees",
                startDate: new DateOnly(2020, 1, 15),
                endDate: new DateOnly(2026, 8, 27),
                title: "Mrs",
                appointedBy: "Appointed by trustees")
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable historicTrusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable);

        Assert.Equal(
            "Trustees who have left within the last 12 months",
            historicTrusteesTable.Caption);

        TableRow row = Assert.Single(historicTrusteesTable.Rows);

        Assert.Collection(
            row.Cells,
            cell => Assert.Equal(
                "Mrs Historic Trustee (Chair of Trustees)",
                cell.Text),
            cell => Assert.Equal("T123", cell.Text),
            cell => Assert.Equal("Appointed by trustees", cell.Text),
            cell => Assert.Equal(
                "15 January 2020 to 27 August 2026",
                cell.Text));
    }

    [Fact]
    public void Map_DoesNotMapTrusteesWhoLeftMoreThanTwelveMonthsAgo()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Historic Trustee",
                role: "Trustee",
                startDate: new DateOnly(2020, 1, 15),
                endDate: new DateOnly(2025, 8, 27))
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable historicTrusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable);

        Assert.Empty(historicTrusteesTable.Rows);
    }

    [Fact]
    public void Map_MapsTrusteeEndingExactlyTwelveMonthsAgoToHistoricTable()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Boundary Trustee",
                role: "Trustee",
                startDate: new DateOnly(2020, 1, 15),
                endDate: new DateOnly(2025, 9, 3))
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable historicTrusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable);

        Assert.Single(historicTrusteesTable.Rows);
    }

    [Fact]
    public void Map_MapsTrusteeEndingTodayToActiveTrusteesTable()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Current Trustee",
                role: "Trustee",
                startDate: new DateOnly(2020, 1, 15),
                endDate: new DateOnly(2026, 9, 3))
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable trusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.TrusteesTable);

        GovUkTable historicTrusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable);

        Assert.Single(trusteesTable.Rows);
        Assert.Empty(historicTrusteesTable.Rows);
    }

    [Fact]
    public void Map_DoesNotMapFutureTrusteeAsActive()
    {
        // Arrange
        GroupReadModel readModel = GroupReadModelTestDoubles.Stub() with
        {
            Trustees =
            [
                TrusteeReadModelTestDoubles.CreateWith(
                id: "T123",
                fullName: "Future Trustee",
                role: "Trustee",
                startDate: new DateOnly(2026, 9, 4),
                endDate: null)
            ]
        };

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        GovUkTable trusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.TrusteesTable);

        GovUkTable historicTrusteesTable =
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable);

        Assert.Empty(trusteesTable.Rows);
        Assert.Empty(historicTrusteesTable.Rows);
    }

    [Fact]
    public void Map_CreatesEmptyTables_WhenCollectionsAreEmpty()
    {
        // Arrange
        GroupReadModel readModel =
            GroupReadModelBuilder.Create()
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

        // Assert
        Assert.Equal("Academies (0)", result.Establishments.Tab);
        Assert.Null(result.Establishments.Table);

        Assert.Empty(
            Assert.IsType<GovUkTable>(
                result.Governance.TrusteesTable).Rows);

        Assert.Empty(
            Assert.IsType<GovUkTable>(
                result.Governance.HistoricTrusteesTable).Rows);

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
            cell => Assert.Equal(
                "Mr Test Trustee (Chair of Trustees)",
                cell.Text),
            cell => Assert.Equal("T123", cell.Text),
            cell => Assert.Equal(
                "Appointed by academy members",
                cell.Text),
            cell => Assert.Equal(
                "15 January 2020 to 15 January 2030",
                cell.Text));
    }

    private static void AssertMembersTable(GovUkTable? table)
    {
        GovUkTable membersTable = Assert.IsType<GovUkTable>(table);

        Assert.Equal("Members", membersTable.Caption);
        AssertGovernanceColumns(membersTable.Columns);

        TableRow row = Assert.Single(membersTable.Rows);

        Assert.Collection(
            row.Cells,
            cell => Assert.Equal("Mrs Test Member", cell.Text),
            cell => Assert.Equal("M123", cell.Text),
            cell => Assert.Equal(
                "Appointed by academy members",
                cell.Text),
            cell => Assert.Equal(
                "From 25 June 2021",
                cell.Text));
    }

    private static void AssertGovernanceColumns(
        IReadOnlyList<TableColumn> columns)
    {
        Assert.Collection(
            columns,
            column =>
            {
                Assert.Equal("Name and role", column.Text);
                Assert.True(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Governor ID", column.Text);
                Assert.False(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Appointed by", column.Text);
                Assert.False(column.IsRowHeader);
            },
            column =>
            {
                Assert.Equal("Term", column.Text);
                Assert.False(column.IsRowHeader);
            });
    }

    private static TableCell GetDetailsValue(GroupDetailsPageViewModel result, string label)
    {
        GovUkTable table = Assert.IsType<GovUkTable>(result.Details.Table);
        TableRow row = Assert.Single(table.Rows, row => row.Cells[0].Text == label);
        return row.Cells[1];
    }
}
