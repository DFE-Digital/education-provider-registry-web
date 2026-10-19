using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments;

public sealed class EstablishmentDetailsBasicDetailsTableMapperTests
{
    private static readonly EstablishmentDetailsBasicDetailsTableMapper Mapper = new();

    [Fact]
    public void Map_Should_Create_Table_Structure()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        Assert.Collection(
            result.Columns,
            column => Assert.True(column.IsRowHeader),
            column => Assert.False(column.IsRowHeader));

        Assert.All(
            result.Rows,
            row =>
            {
                Assert.Equal(2, row.Cells.Count);
                Assert.True(row.Cells[0].IsBold);
            });
    }

    [Fact]
    public void Map_Should_Map_Empty_Status_When_Status_Is_Null()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(null!)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Status",
            string.Empty);
    }

    [Theory]
    [InlineData(EstablishmentStatus.Open, "Open")]
    [InlineData(EstablishmentStatus.Closed, "Closed")]
    [InlineData(EstablishmentStatus.OpenButProposedToClose, "Open but proposed to close")]
    [InlineData(EstablishmentStatus.ProposedToOpen, "Proposed to open")]
    public void Map_Should_Map_Open_Status(EstablishmentStatus status, string expectedLabel)
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(status)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(result, "Status", expectedLabel);
    }

    [Fact]
    public void Map_Should_Map_LocalAuthority()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithLocalAuthorityName("Test Local Authority")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Local authority",
            "Test Local Authority");
    }

    [Fact]
    public void Map_Should_Omit_LocalAuthority_When_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Local authority"));
    }

    [Fact]
    public void Map_Should_Map_Identifier_Rows()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithUrn(123456)
                .WithUkprn("testUkprn")
                .WithDfeNumber("123/1234")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        TableCell idNumbersCell =
            GetValueCell(result, "ID numbers");

        Assert.Collection(
            idNumbersCell.Rows,
            row => AssertSubRow(row, "LAESTAB", "1231234"),
            row => AssertSubRow(row, "UKPRN", "testUkprn"),
            row => AssertSubRow(row, "URN", "123456"));
    }

    [Fact]
    public void Map_Should_Map_Address()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithSiteName("Site")
                .WithAddressLine1("Line 1")
                .WithAddressLine2("Line 2")
                .WithTown("Town")
                .WithCounty("County")
                .WithPostcode("AB1 2CD")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Address",
            "Line 1, Line 2, Town, County, AB1 2CD");
    }

    [Fact]
    public void Map_Should_Map_Group_Link()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithGroupName("testGroupName")
                .WithGroupCode("testGroupCode")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        TableCell groupCell =
            GetValueCell(result, "Part of group");

        Assert.Equal("Testgroupname", groupCell.Text);
        Assert.Equal("/groups/testGroupCode", groupCell.Href);
    }

    [Fact]
    public void Map_Should_Map_Contact_Details()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithWebsite("testWebsite")
                .WithTelephoneNumber("0123456789")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        TableCell websiteCell = GetValueCell(result, "Website");

        Assert.Equal("testWebsite", websiteCell.Text);
        Assert.True(websiteCell.OpenInNewTab);

        AssertValue(
            result,
            "Telephone number",
            "0123456789");
    }

    [Fact]
    public void Map_Should_Map_Empty_Ofsted_When_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        TableCell ofstedCell =
            GetValueCell(result, "Ofsted");

        Assert.Equal(string.Empty, ofstedCell.Text);
        Assert.Equal(string.Empty, ofstedCell.Href);
        Assert.True(ofstedCell.OpenInNewTab);
    }

    [Fact]
    public void Map_Should_Map_Ofsted_Link()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithOfstedDate(new DateOnly(2024, 5, 31))
                .WithOfstedReportUrl("https://ofsted.example.com")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        TableCell ofstedCell =
            GetValueCell(result, "Ofsted");

        Assert.Equal(
            "Latest report 31 May 2024 (opens in new tab)",
            ofstedCell.Text);

        Assert.Equal(
            "https://ofsted.example.com",
            ofstedCell.Href);

        Assert.True(ofstedCell.OpenInNewTab);
    }

    [Fact]
    public void Map_Should_Omit_Headteacher_When_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithHeadteacher(null)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Headteacher"));
    }

    [Fact]
    public void Map_Should_Omit_Group_When_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Part of group"));
    }

    [Fact]
    public void Map_Should_Omit_Contact_Details_When_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Website"));

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Telephone number"));
    }

    [Fact]
    public void Map_Should_Map_NotRecorded_When_SenProvision_Is_Null()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithSenProvision(null)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Type of SEN provision",
            "Not recorded");
    }

    [Fact]
    public void Map_Should_Map_Type()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithType("Academy")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Type",
            "Academy");
    }

    [Fact]
    public void Map_Should_Map_Phase_Of_Education()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithPhase("Primary")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Phase of education",
            "Primary");
    }

    [Fact]
    public void Map_Should_Map_Headteacher()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithHeadteacher("Jane Smith")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Headteacher",
            "Jane Smith");
    }

    [Fact]
    public void Map_Should_Map_Age_Range()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithAgeRange(4, 11)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Age range",
            "4 to 11");
    }

    [Fact]
    public void Map_Should_Map_Gender()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithGender("Mixed")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Gender",
            "Mixed");
    }

    [Fact]
    public void Map_Should_Map_Number_Of_Pupils()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithPupilCount(123)
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Number of pupils",
            "123");
    }

    [Fact]
    public void Map_Should_Map_Religious_Character()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithReligiousCharacter("Church of England")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Religious character",
            "Church of England");
    }

    [Fact]
    public void Map_Should_Map_Sen_Provision()
    {
        // Arrange
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithSenProvision("SEN Unit")
                .Build();

        // Act
        GovUkTable result = Mapper.Map(model);

        // Assert
        AssertValue(
            result,
            "Type of SEN provision",
            "SEN Unit");
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
