using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.FilterExpressions;

public sealed class SingleOrMultiValueEqualsExpression : ISearchFilterExpression
{
    public string GetFilterExpression(SearchFilterRequest searchFilterRequest)
    {
        ArgumentNullException.ThrowIfNull(searchFilterRequest);

        List<string> expressions =
            BuildExpressions(
                column: searchFilterRequest.FilterKey,
                values: searchFilterRequest.FilterValues);

        if (expressions.Count == 0){
            return string.Empty;
        }

        if (expressions.Count == 1){
            return expressions[0];
        }

        return string.Concat(
            LogicalOperators.Open,
            string.Join(LogicalOperators.Or, expressions),
            LogicalOperators.Close
        );
    }

    private static List<string> BuildExpressions(string column, object[] values)
    {
        List<string> expressions = new(values.Length);

        for (int i = 0; i < values.Length; i++)
        {
            object formatted = FormatValue(values[i]);

            if (ReferenceEquals(formatted, NoValue.Instance))
            {
                continue;
            }

            expressions.Add(
                string.Concat(column, LogicalOperators.Eq, (string)formatted));
        }

        return expressions;
    }

    private static object FormatValue(object value)
    {
        return value switch
        {
            null => NoValue.Instance,

            string s when string.IsNullOrWhiteSpace(s)
                => NoValue.Instance,

            string s
                => LogicalOperators.Quote
                   + s.Replace(LogicalOperators.Quote, LogicalOperators.Quote + LogicalOperators.Quote)
                   + LogicalOperators.Quote,

            bool b
                => b ? LogicalOperators.TrueLiteral : LogicalOperators.FalseLiteral,

            _ => FormatNonString(value)
        };
    }

    private static object FormatNonString(object value) =>
        (value is null || string.IsNullOrWhiteSpace(value.ToString())) ? NoValue.Instance : value;

    private sealed class NoValue
    {
        public static readonly NoValue Instance = new();
        private NoValue() { }
    }

    private readonly struct LogicalOperators
    {
        public const string Or = " OR ";
        public const string Eq = " = ";
        public const string Open = "(";
        public const string Close = ")";
        public const string Quote = "'";
        public const string TrueLiteral = "TRUE";
        public const string FalseLiteral = "FALSE";
    }
}
