using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;

[ApiController]
[Route("_test/antiforgery")]
public sealed class AntiforgeryTokenController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryTokenController(
        IAntiforgery antiforgery)
    {
        ArgumentNullException.ThrowIfNull(antiforgery);
        _antiforgery = antiforgery;
    }

    [HttpGet]
    public ActionResult<AntiForgeryRequestToken> Get()
    {
        AntiforgeryTokenSet tokens =
            _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(
            new AntiForgeryRequestToken(
                tokens.FormFieldName!,
                tokens.RequestToken!));
    }
}

public sealed record AntiForgeryRequestToken(
    string FormFieldName,
    string RequestToken);