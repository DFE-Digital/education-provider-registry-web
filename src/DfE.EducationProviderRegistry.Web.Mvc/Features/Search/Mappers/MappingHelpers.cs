using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public static class MappingHelpers
{
    internal static string? CreateLinkUrl(string prefix, string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : prefix + value;
    }

    internal static string? CreateWebsiteUrl(string? website)
    {
        if (string.IsNullOrWhiteSpace(website))
        {
            return null;
        }

        website = website.Trim();

        if (website.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            website.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return website;
        }

        return $"https://{website}";
    }

    public static string CombineAddress(
        SiteAddressModel? siteAddress,
        string? establishmentName)
    {
        if (siteAddress is null)
        {
            return string.Empty;
        }

        IEnumerable<string?> addressParts =
        [
            siteAddress.AddressLine1,
            siteAddress.AddressLine2,
            siteAddress.Town,
            siteAddress.County,
            siteAddress.Postcode
        ];

        string address = string.Join(", ",
            addressParts.Where(value =>
                !string.IsNullOrWhiteSpace(value) &&
                !string.Equals(
                    value,
                    establishmentName,
                    StringComparison.OrdinalIgnoreCase)));

        return string.IsNullOrEmpty(address) ? string.Empty : address;
    }
}