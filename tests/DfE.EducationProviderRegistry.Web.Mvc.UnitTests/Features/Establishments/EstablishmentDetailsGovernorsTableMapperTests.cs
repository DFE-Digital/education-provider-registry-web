using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments.Mappers;

public sealed class EstablishmentDetailsGovernorsTableMapperTests
{
    private static readonly EstablishmentDetailsGovernorsTableMapper Mapper = new();

    [Fact]
    public void Map_CreatesExpectedTableStructure()
    {
        // Arrange
        GovernorModel[] governors =
        [
            CreateGovernor()
        ];

        // Act
        GovUkTable table = Mapper.Map(governors);

        // Assert
        Assert.Equal("Governors", table.Caption);

        Assert.Collection(
            table.Columns,
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

    [Fact]
    public void Map_MapsGovernorsInInputOrder()
    {
        // Arrange
        GovernorModel[] governors =
        [
            CreateGovernor(
                identifier: "testGovernorIdentifier1",
                name: "testGovernorName1"),
            CreateGovernor(
                identifier: "testGovernorIdentifier2",
                name: "testGovernorName2")
        ];

        // Act
        GovUkTable table = Mapper.Map(governors);

        // Assert
        Assert.Collection(
            table.Rows,
            row => AssertGovernorRow(
                row,
                expectedName: "testGovernorName1",
                expectedIdentifier: "testGovernorIdentifier1"),
            row => AssertGovernorRow(
                row,
                expectedName: "testGovernorName2",
                expectedIdentifier: "testGovernorIdentifier2"));
    }

    [Fact]
    public void Map_UsesEmptyString_WhenGovernorIdentifierIsNull()
    {
        // Arrange
        GovernorModel[] governors =
        [
            CreateGovernor(
                identifier: null,
                name: "testGovernorName")
        ];

        // Act
        GovUkTable table = Mapper.Map(governors);

        // Assert
        TableRow row = Assert.Single(table.Rows);

        AssertGovernorRow(
            row,
            expectedName: "testGovernorName",
            expectedIdentifier: string.Empty);
    }

    [Fact]
    public void Map_UsesEmptyStringForStartDate()
    {
        // Arrange
        GovernorModel[] governors =
        [
            CreateGovernor()
        ];

        // Act
        GovUkTable table = Mapper.Map(governors);

        // Assert
        TableRow row = Assert.Single(table.Rows);

        Assert.Equal(string.Empty, row.Cells[2].Text);
    }

    [Fact]
    public void Map_ReturnsEmptyTable_WhenNoGovernorsAreProvided()
    {
        // Arrange
        GovernorModel[] governors = [];

        // Act
        GovUkTable table = Mapper.Map(governors);

        // Assert
        Assert.Equal("Governors", table.Caption);
        Assert.Empty(table.Rows);
        Assert.Equal(3, table.Columns.Count);
    }

    private static GovernorModel CreateGovernor(
        string? identifier = "testGovernorIdentifier",
        string name = "testGovernorName")
    {
        return new GovernorModel(
            Identifier: new GovernanceIdentifier(identifier),
            Name: new Name(name));
    }

    private static void AssertGovernorRow(
        TableRow row,
        string expectedName,
        string expectedIdentifier)
    {
        Assert.Collection(
            row.Cells,
            cell => Assert.Equal(expectedName, cell.Text),
            cell => Assert.Equal(expectedIdentifier, cell.Text),
            cell => Assert.Equal(string.Empty, cell.Text));
    }
}