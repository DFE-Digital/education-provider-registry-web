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
    public void Map_Should_Omit_Status_When_Status_Is_Null()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(null!)
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Status"));
    }

    [Fact]
    public void Map_Should_Map_Open_Status_With_Opened_Date()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.Open)
                .WithOpenedDate(new DateOnly(2024, 1, 15))
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Status",
            "Open on 15 January 2024");
    }

    [Fact]
    public void Map_Should_Map_Proposed_To_Open_Status_With_Opened_Date()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.ProposedToOpen)
                .WithOpenedDate(new DateOnly(2024, 2, 1))
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Status",
            "Proposed to open on 01 February 2024");
    }

    [Fact]
    public void Map_Should_Map_Closed_Status_With_Closed_Date()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.Closed)
                .WithClosedDate(new DateOnly(2024, 3, 10))
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Status",
            "Closed on 10 March 2024");
    }

    [Fact]
    public void Map_Should_Map_Open_But_Proposed_To_Close_Status_With_Closed_Date()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.OpenButProposedToClose)
                .WithClosedDate(new DateOnly(2024, 4, 5))
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Status",
            "Open but proposed to close on 05 April 2024");
    }

    [Fact]
    public void Map_Should_Omit_Sen_Provision_When_Null()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithSenProvision(null)
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Type of SEN provision"));
    }

    [Fact]
    public void Map_Should_Omit_Ofsted_When_Not_Eligible()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Ofsted"));
    }

    [Fact]
    public void Map_Should_Map_Ofsted_Link()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.Open)
                .WithOpenedDate(new DateOnly(2024, 1, 1))
                .WithType("Academy")
                .WithUrn(123456)
                .Build();

        GovUkTable result = Mapper.Map(model);

        TableCell ofstedCell =
            GetValueCell(result, "Ofsted");

        Assert.Equal(
            "View Ofsted report",
            ofstedCell.Text);

        Assert.Equal(
            "https://www.ofsted.gov.uk/oxedu_providers/full/(urn)/123456",
            ofstedCell.Href);

        Assert.True(ofstedCell.OpenInNewTab);
    }

    [Fact]
    public void Map_Should_Map_Number_Of_Pupils()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithPupilGirlCount(1)
                .WithPupilBoyCount(1)
                .WithPupilCount(2)
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Number of pupils",
            "2 (1 boys, 1 girls)");
    }

    [Fact]
    public void Map_Should_Map_Free_School_Meals()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithFreeSchoolMealsNumber(50)
                .WithFreeSchoolMealsPercentage(25)
                .Build();

        GovUkTable result = Mapper.Map(model);

        AssertValue(
            result,
            "Pupils eligible for free school meals",
            "50 (25%)");
    }

    [Theory]
    [InlineData("None")]
    [InlineData("Does not apply")]
    public void Map_Should_Omit_Religious_Character_When_Excluded(
    string value)
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithReligiousCharacter(value)
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Religious character"));
    }

    [Fact]
    public void Map_Should_Omit_Diocese_When_Not_Applicable()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithDiocese("Not applicable")
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Diocese"));
    }

    [Fact]
    public void Map_Should_Omit_Ofsted_For_Other_Independent_School()
    {
        EstablishmentDetailsModel model =
            EstablishmentDetailsBuilder.Create()
                .WithStatus(EstablishmentStatus.Open)
                .WithType("Other independent school")
                .Build();

        GovUkTable result = Mapper.Map(model);

        Assert.Null(
            result.Rows.SingleOrDefault(
                row => row.Cells[0].Text == "Ofsted"));
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
                .WithGroupUid("testGroupCode")
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
