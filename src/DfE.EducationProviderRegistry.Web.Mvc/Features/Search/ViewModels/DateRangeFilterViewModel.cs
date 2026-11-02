using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class DateRangeFilterViewModel : IValidatableObject
{
    public DateInputViewModel From { get; set; } = new();
    public DateInputViewModel To { get; set; } = new();

    public bool IsEmpty => From.IsEmpty && To.IsEmpty;


    public bool TryGetRange(out DateOnly from, out DateOnly to)
    {
        bool validFrom = From.TryGetDate(out from);
        bool validateTo = To.TryGetDate(out to);
        return validFrom && validateTo && from <= to;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        DateOnly earliestDate = new(1800, 1, 1);

        bool hasFrom = From.TryGetDate(out DateOnly from);
        bool hasTo = To.TryGetDate(out DateOnly to);

        if (!From.IsEmpty && !hasFrom)
        {
            yield return new ValidationResult(
                "'From' must be a real date.",
                [nameof(From)]);
        }

        else if (hasFrom && from < earliestDate)
        {
            yield return new ValidationResult(
                $"'From' must be after {earliestDate:dd/MM/yyyy}.",
                [nameof(From)]);
        }

        if (!To.IsEmpty && !hasTo)
        {
            yield return new ValidationResult(
                "'To' must be a real date.",
                [nameof(To)]);
        }
        else if (hasTo && to < earliestDate)
        {
            yield return new ValidationResult(
                $"'To' must be after {earliestDate:dd/MM/yyyy}.",
                [nameof(To)]);
        }

        if (hasFrom && hasTo &&
            from >= earliestDate &&
            to >= earliestDate &&
            to < from)
        {
            yield return new ValidationResult(
                "'To' date must be the same as or after 'From' date.",
                [nameof(To)]);
        }

    }
}
