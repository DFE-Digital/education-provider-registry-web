namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Controllers.TestDoubles;

public static class CookieHeaderParserTestDouble
{
    public static string ExtractJson(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
        {
            throw new InvalidOperationException("Set-Cookie header was not written.");
        }

        // First segment: cookies_policy=<value>
        string firstPart = header.Split(';')[0];
        string[] parts = firstPart.Split('=');

        if (parts.Length < 2)
        {
            throw new InvalidOperationException("Cookie header format invalid.");
        }

        string encoded = parts[1];
        return Uri.UnescapeDataString(encoded);
    }
}

