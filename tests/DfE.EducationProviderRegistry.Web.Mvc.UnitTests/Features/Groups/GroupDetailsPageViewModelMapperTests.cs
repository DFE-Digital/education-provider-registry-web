using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
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
        TableCell value = GetDetailsValue(result, "Status");
        Assert.Equal("testStatus", value.Text);
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
              .WithGroupType(type)
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
                .WithGroupType("Single-Academy trust")
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
        GroupReadModel readModel =
            GroupReadModelBuilder.Create()
              .Build();

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

    private static TableCell GetDetailsValue(GroupDetailsPageViewModel result, string label)
    {
        GovUkTable table = Assert.IsType<GovUkTable>(result.Details.Table);
        TableRow row = Assert.Single(table.Rows, row => row.Cells[0].Text == label);
        return row.Cells[1];
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
