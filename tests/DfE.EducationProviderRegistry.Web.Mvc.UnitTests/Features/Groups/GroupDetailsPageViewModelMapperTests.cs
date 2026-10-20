using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups.TestDoubles;
using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using GroupReadModelTestDoubles = DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails.GroupReadModelTestDoubles;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Groups;

public sealed class GroupDetailsPageViewModelMapperTests
{
    private readonly GroupDetailsPageViewModelMapper _mapper;

    public GroupDetailsPageViewModelMapperTests()
    {
        _mapper = new();
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
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        Assert.Equal("Test Group", result.Heading);
        Assert.Equal("Details", result.Details.Tab);

        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        AssertSummaryRow(summary.Rows[0], "UID", "123");
        AssertSummaryRow(summary.Rows[1], "Group ID", "group-1");
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Null(summary.Rows.SingleOrDefault((row) => row.Key == "UKPRN"));
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        SummaryListRow row = Assert.Single(summary.Rows, (row) => row.Key == "UKPRN");
        Assert.Equal("testUkprn", row.Value.Text);
        Assert.Null(row.Value.Href);
        Assert.Empty(row.Actions);
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Null(summary.Rows.SingleOrDefault((row) => row.Key == "Company number"));
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        SummaryListRow row = Assert.Single(summary.Rows, (row) => row.Key == "Company number");
        Assert.Equal("testCompaniesHouseId (opens in new tab)", row.Value.Text);
        Assert.Equal("https://find-and-update.company-information.service.gov.uk/company/testCompaniesHouseId", row.Value.Href);
        Assert.Empty(row.Actions);
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Null(summary.Rows.SingleOrDefault((row) => row.Key == "Status"));
    }

    [Fact]
    public void Map_Maps_Status()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithStatus("testStatus")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        SummaryListRow row = Assert.Single(summary.Rows, (row) => row.Key == "Status");
        Assert.Equal("testStatus", row.Value.Text);
        Assert.Null(row.Value.Href);
        Assert.Empty(row.Actions);
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Null(summary.Rows.SingleOrDefault((row) => row.Key == "Address"));
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
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        SummaryListRow row = Assert.Single(summary.Rows, (row) => row.Key == "Address");
        Assert.Equal("testAddress, testVille, TE5 1ST", row.Value.Text);
        Assert.Null(row.Value.Href);
        Assert.Empty(row.Actions);
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
              .WithGroupType(type)
              .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        Assert.Null(summary.Rows.SingleOrDefault((row) => row.Key == "Type"));
    }

    [Fact]
    public void Map_Maps_GroupType()
    {
        // Arrange
        GroupReadModel model =
            GroupReadModelBuilder.Create()
                .WithGroupType("Single-Academy trust")
                .Build();

        // Act
        GroupDetailsPageViewModel result = _mapper.Map(model);

        // Assert
        GovUkSummaryList summary =
            Assert.IsType<GovUkSummaryList>(result.Details.Summary);

        SummaryListRow row = Assert.Single(summary.Rows, (row) => row.Key == "Type");
        Assert.Equal("Single-Academy trust", row.Value.Text);
        Assert.Null(row.Value.Href);
        Assert.Empty(row.Actions);
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
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

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
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

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
        GroupDetailsPageViewModel result = _mapper.Map(readModel);

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
