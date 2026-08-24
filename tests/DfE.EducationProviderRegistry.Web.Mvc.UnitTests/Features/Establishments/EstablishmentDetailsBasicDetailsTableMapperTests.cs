using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments.Mappers;

public sealed class EstablishmentDetailsBasicDetailsTableMapperTests
{
    private static readonly EstablishmentDetailsBasicDetailsTableMapper Mapper = new();

    [Fact]
    public void Map_CreatesExpectedTableStructure()
    {
        // Arrange
        EstablishmentDetailsModel dto = CreateModel();

        // Act
        GovUkTable table = Mapper.Map(dto);

        // Assert
        Assert.Collection(
            table.Columns,
            column => Assert.True(column.IsRowHeader),
            column => Assert.False(column.IsRowHeader));

        string?[] expectedLabels =
        [
            "Status",
            "ID numbers",
            "Headteacher",
            "Type",
            "Phase of education",
            "Address",
            "Local authority",
            "Part of",
            "Age range",
            "Gender",
            "Number of pupils",
            "Pupils capacity",
            "Religious character",
            "Type of SEN provision",
            "Website",
            "Ofsted",
            "School profiles service"
        ];

        Assert.Equal(
            expectedLabels,
            table.Rows
                .Select(row => row.Cells[0].Text)
                .ToArray());

        Assert.All(
            table.Rows,
            row =>
            {
                Assert.Equal(2, row.Cells.Count);
                Assert.True(row.Cells[0].IsBold);
            });
    }

    [Fact]
    public void Map_MapsBasicDetailValues()
    {
        // Arrange
        EstablishmentDetailsModel dto = CreateModel();

        // Act
        GovUkTable table = Mapper.Map(dto);

        // Assert
        AssertValue(table, "Status", "testStatus");
        AssertValue(table, "Headteacher", "testHeadteacher");
        AssertValue(table, "Type", "testType");
        AssertValue(table, "Phase of education", "testPhase");
        AssertValue(table, "Address", "testAddressLine1");
        AssertValue(table, "Local authority", "testLocalAuthority");
        AssertValue(table, "Part of", "testGroupName");
        AssertValue(table, "Age range", "testAgeRange");
        AssertValue(table, "Gender", "Yet to be provisioned");
        AssertValue(table, "Number of pupils", "Yet to be provisioned");
        AssertValue(table, "Pupils capacity", "testAgeRange");
        AssertValue(
            table,
            "Religious character",
            "testReligiousCharacter");
        AssertValue(
            table,
            "Type of SEN provision",
            "testSenProvision");
        AssertValue(table, "Website", "testWebsite");
        AssertValue(
            table,
            "School profiles service",
            "Yet to be provisioned");
    }

    [Fact]
    public void Map_MapsIdNumberSubRows()
    {
        // Arrange
        EstablishmentDetailsModel dto = CreateModel();

        // Act
        GovUkTable table = Mapper.Map(dto);

        // Assert
        TableCell idNumbersCell = GetValueCell(
            table,
            "ID numbers");

        Assert.Collection(
            idNumbersCell.Rows,
            row => AssertSubRow(
                row,
                expectedLabel: "LAESTAB",
                expectedValue: "testEstablishmentNumber"),
            row => AssertSubRow(
                row,
                expectedLabel: "UKPRN",
                expectedValue: "testEstablishmentNumber"),
            row => AssertSubRow(
                row,
                expectedLabel: "URN",
                expectedValue: "123456"));
    }

    [Fact]
    public void Map_MapsOfstedLink()
    {
        // Arrange
        EstablishmentDetailsModel dto = CreateModel();

        // Act
        GovUkTable table = Mapper.Map(dto);

        // Assert
        TableCell ofstedCell = GetValueCell(
            table,
            "Ofsted");

        Assert.Equal(
            "Latest report 31 May 2024 (opens in new tab)",
            ofstedCell.Text);

        Assert.Equal(
            "testInspectionOutcome",
            ofstedCell.Href);

        Assert.True(ofstedCell.OpenInNewTab);
    }

    [Fact]
    public void Map_UsesEmptyStrings_WhenOptionalValuesAreMissing()
    {
        // Arrange
        EstablishmentDetailsModel dto = CreateModel() with
        {
            Number = new EstablishmentNumberModel(null),
            Headteacher = null,
            Phase = new PhaseOfEducationModel(null),
            Address = null,
            LocalAuthority = null,
            GroupName = null,
            AgeRange = null,
            ReligiousCharacter = null,
            SenProvision = null,
            ContactDetails = null,
            Ofsted = null
        };

        // Act
        GovUkTable table = Mapper.Map(dto);

        // Assert
        AssertValue(table, "Headteacher", string.Empty);
        AssertValue(table, "Phase of education", string.Empty);
        AssertValue(table, "Address", string.Empty);
        AssertValue(table, "Local authority", string.Empty);
        AssertValue(table, "Part of", string.Empty);
        AssertValue(table, "Age range", string.Empty);
        AssertValue(table, "Pupils capacity", string.Empty);
        AssertValue(table, "Religious character", string.Empty);
        AssertValue(table, "Type of SEN provision", string.Empty);
        AssertValue(table, "Website", string.Empty);

        TableCell idNumbersCell = GetValueCell(
            table,
            "ID numbers");

        Assert.Collection(
            idNumbersCell.Rows,
            row => AssertSubRow(
                row,
                "LAESTAB",
                string.Empty),
            row => AssertSubRow(
                row,
                "UKPRN",
                string.Empty),
            row => AssertSubRow(
                row,
                "URN",
                "123456"));

        TableCell ofstedCell = GetValueCell(
            table,
            "Ofsted");

        Assert.Equal(string.Empty, ofstedCell.Text);
        Assert.Equal(string.Empty, ofstedCell.Href);
        Assert.True(ofstedCell.OpenInNewTab);
    }

    private static EstablishmentDetailsModel CreateModel()
    {
        return new EstablishmentDetailsModel
        {
            Urn = EstablishmentUrnModel.Create("123456"),
            Number = new EstablishmentNumberModel(
                "testEstablishmentNumber"),
            Status = new EstablishmentStatusModel(
                "testStatus"),
            Type = new EstablishmentTypeModel(
                "testType"),
            Phase = new PhaseOfEducationModel(
                "testPhase"),
            Uid = "testUid",
            Headteacher = "testHeadteacher",
            Address = new SiteAddressModel(
                Name: "testSiteName",
                AddressLine1: "testAddressLine1",
                AddressLine2: "testAddressLine2",
                Town: "testTown",
                County: "testCounty",
                Postcode: "testPostcode"),
            LocalAuthority = new LocalAuthority(
                localAuthorityName: "testLocalAuthority",
                localAuthorityCode: "testLocalAuthorityCode"),
            GroupName = "testGroupName",
            AgeRange = "testAgeRange",
            Gender = "testGender",
            ReligiousCharacter = "testReligiousCharacter",
            SenProvision = "testSenProvision",
            ContactDetails = new EstablishmentContactDetails(
                Website: "testWebsite",
                TelephoneNumber: "testTelephoneNumber"),
            Ofsted = new EstablishmentInspection
            {
                InspectionDate = new DateOnly(2024, 5, 31),
                InspectionOutcome = "testInspectionOutcome"
            }
        };
    }

    private static void AssertValue(
        GovUkTable table,
        string label,
        string? expectedValue)
    {
        TableCell valueCell = GetValueCell(
            table,
            label);

        Assert.Equal(expectedValue, valueCell.Text);
    }

    private static TableCell GetValueCell(
        GovUkTable table,
        string label)
    {
        TableRow row = table.Rows.Single(
            candidate => candidate.Cells[0].Text == label);

        return row.Cells[1];
    }

    private static void AssertSubRow(
        TableCellRow row,
        string expectedLabel,
        string expectedValue)
    {
        Assert.Equal(expectedLabel, row.Label.Text);
        Assert.Equal(expectedValue, row.Value.Text);
    }
}