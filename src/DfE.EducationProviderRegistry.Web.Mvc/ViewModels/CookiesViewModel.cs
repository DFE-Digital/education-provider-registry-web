namespace DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

public sealed record CookiesViewModel(
    bool? Analytics = null,
    bool Saved = false);