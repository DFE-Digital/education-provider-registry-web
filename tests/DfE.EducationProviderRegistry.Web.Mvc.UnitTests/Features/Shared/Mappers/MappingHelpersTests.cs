using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Mappers;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Mappers;

public class MappingHelpersTests
{
    public class CreateLinkUrl
    {
        [Theory]
        [InlineData("https://example.com/", "123", "https://example.com/123")]
        [InlineData("mailto:", "test@example.com", "mailto:test@example.com")]
        [InlineData("tel:", "01234567890", "tel:01234567890")]
        public void GivenValue_ReturnsPrefixAndValue(
            string prefix,
            string value,
            string expected)
        {
            string? result = MappingHelpers.CreateLinkUrl(prefix, value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void GivenNullOrWhiteSpaceValue_ReturnsNull(string? value)
        {
            string? result = MappingHelpers.CreateLinkUrl("https://example.com/", value);

            Assert.Null(result);
        }
    }

    public class CreateWebsiteUrl
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void GivenNullOrWhiteSpaceWebsite_ReturnsNull(string? website)
        {
            string? result = MappingHelpers.CreateWebsiteUrl(website);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("www.princefield.staffs.sch.uk/",
            "https://www.princefield.staffs.sch.uk/")]
        [InlineData("www.petersgateinfantschool.co.uk",
            "https://www.petersgateinfantschool.co.uk")]
        [InlineData("sjb.solihull.sch.uk",
            "https://sjb.solihull.sch.uk")]
        [InlineData("yattonschools.co.uk",
            "https://yattonschools.co.uk")]
        public void GivenWebsiteWithoutScheme_PrefixesWithHttps(
            string noPrefixWebsite,
            string expected)
        {
            string? result = MappingHelpers.CreateWebsiteUrl(noPrefixWebsite);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("http://www.lanfranc.org.uk",
            "http://www.lanfranc.org.uk")]
        [InlineData("HTTP://www.lanfranc.org.uk",
            "HTTP://www.lanfranc.org.uk")]
        public void GivenWebsiteWithHttpScheme_ReturnsWebsiteUnmodified(
            string httpPrefixWebsite,
            string expected)
        {
            string? result = MappingHelpers.CreateWebsiteUrl(httpPrefixWebsite);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(
            "https://www.armthorpeacademy.org.uk/",
            "https://www.armthorpeacademy.org.uk/")]
        [InlineData(
            "HTTPS://www.armthorpeacademy.org.uk/",
            "HTTPS://www.armthorpeacademy.org.uk/")]
        public void GivenWebsiteWithHttpsScheme_ReturnsWebsiteUnmodified(
            string httpsPrefixWebsite,
            string expected)
        {
            string? result = MappingHelpers.CreateWebsiteUrl(httpsPrefixWebsite);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(
            "  www.example.com  ",
            "https://www.example.com")]
        [InlineData(
            "  http://www.example.com  ",
            "http://www.example.com")]
        [InlineData(
            "  https://www.example.com  ",
            "https://www.example.com")]
        public void GivenWebsiteWithSurroundingWhitespace_TrimsWebsite(
            string whitespaceWebsite,
            string expected)
        {
            string? result = MappingHelpers.CreateWebsiteUrl(whitespaceWebsite);

            Assert.Equal(expected, result);
        }
    }

    public class CombineAddress
    {
        [Fact]
        public void GivenNullSiteAddress_ReturnsEmptyString()
        {
            string result = MappingHelpers.CombineAddress(null, null);

            Assert.Empty(result);
        }

        [Fact]
        public void CombineAddress_WhenSiteAddressIsNull_ReturnsEmptyString()
        {
            // Act
            string result = MappingHelpers.CombineAddress(null, "Test School");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CombineAddress_WhenAllAddressPartsAreProvided_ReturnsCombinedAddress()
        {
            // Arrange
            SiteAddressModel address = new(
                "Test School",
                "1 Test Street",
                "Test Area",
                "Test Town",
                "Test County",
                "TE1 1ST");

            // Act
            string result = MappingHelpers.CombineAddress(address, "Test School");

            // Assert
            Assert.Equal(
                "1 Test Street, Test Area, Test Town, Test County, TE1 1ST",
                result);
        }

        [Fact]
        public void CombineAddress_WhenAddressPartsAreEmpty_ExcludesThem()
        {
            // Arrange
            SiteAddressModel address = new SiteAddressModel(
                "Test School",
                "1 Test Street",
                string.Empty,
                string.Empty,
                "Test County",
                "TE1 1ST");

            // Act
            string result = MappingHelpers.CombineAddress(address, "Test School");

            // Assert
            Assert.Equal(
                "1 Test Street, Test County, TE1 1ST",
                result);
        }

        [Fact]
        public void CombineAddress_WhenAddressContainsEstablishmentName_ExcludesEstablishmentName()
        {
            // Arrange
            SiteAddressModel address = new SiteAddressModel(
                "Test School",
                "Test School",
                "1 Test Street",
                "Test Town",
                "Test County",
                "TE1 1ST");

            // Act
            string result = MappingHelpers.CombineAddress(address, "Test School");

            // Assert
            Assert.Equal(
                "1 Test Street, Test Town, Test County, TE1 1ST",
                result);
        }

        [Fact]
        public void CombineAddress_WhenEstablishmentNameHasDifferentCasing_ExcludesEstablishmentName()
        {
            // Arrange
            SiteAddressModel address = new SiteAddressModel(
                "Test School",
                "TEST SCHOOL",
                "1 Test Street",
                "Test Town",
                "Test County",
                "TE1 1ST");

            // Act
            string result = MappingHelpers.CombineAddress(address, "Test School");

            // Assert
            Assert.Equal(
                "1 Test Street, Test Town, Test County, TE1 1ST",
                result);
        }

        [Fact]
        public void CombineAddress_WhenNoValidAddressPartsExist_ReturnsEmptyString()
        {
            // Arrange
            SiteAddressModel address = new SiteAddressModel(
                "Test School",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            // Act
            string result = MappingHelpers.CombineAddress(address, "Test School");

            // Assert
            Assert.Empty(result);
        }
    }
}