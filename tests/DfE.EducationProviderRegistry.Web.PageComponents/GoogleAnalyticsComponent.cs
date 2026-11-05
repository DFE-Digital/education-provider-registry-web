using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents;

public sealed class GoogleAnalyticsComponent
{
    private readonly ISearchContext _context;
    private readonly Uri _analyticsDomain;

    public GoogleAnalyticsComponent(ISearchContext context, Uri analyticsDomain)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(analyticsDomain);
        _context = context;
        _analyticsDomain = analyticsDomain;
    }

    public bool Exists()
    {
        return TagManagerJavascriptTag is not null &&
            NoJavascriptTag is not null;
    }
    private IElement? TagManagerJavascriptTag =>
        _context.FindAll("script")
            .SingleOrDefault((scriptTag) =>
                scriptTag.Text.Contains(_analyticsDomain.ToString()));

    private IElement? NoJavascriptTag => _context.Find($"noscript iframe[title='Google Tag Manager']");
}
