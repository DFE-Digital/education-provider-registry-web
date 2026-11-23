using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.ViewModels.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NonEmptyListAttribute(
    string errorMessage = "Select at least one dataset.")
    : ValidationAttribute(errorMessage)
{
    public override bool IsValid(object? value) => value switch
    {
        null or string => false,
        ICollection collection => collection.Count > 0,
        IEnumerable enumerable => HasAnyElement(enumerable),
        _ => false
    };

    private static bool HasAnyElement(IEnumerable enumerable)
    {
        IEnumerator enumerator = enumerable.GetEnumerator();
        using IDisposable? disposable = enumerator as IDisposable;
        return enumerator.MoveNext();
    }
}
