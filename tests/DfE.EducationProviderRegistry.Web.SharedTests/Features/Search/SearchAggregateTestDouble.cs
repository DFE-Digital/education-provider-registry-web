using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

public static class SearchAggregateTestDouble
{
    public static IReadOnlyCollection<SearchAggregate> CreateResults(
        int count = 10,
        string namePrefix = "school")
    {
        List<SearchAggregate> output = [];

        for (int index = 0; index < count; index++)
        {
            bool isMulti = index % 2 == 0;

            SearchAggregateBuilder builder =
                SearchAggregateBuilder.Create()
                    .WithProviderName($"{namePrefix} {index}")
                    .WithProviderTypeId(isMulti ? 1L : 2L)
                    .WithProviderTypeName(isMulti ? "Mutli-Academy Trust" : "Single-Academy Trust");

            SearchAggregate model = builder.Build();
            output.Add(model);
        }

        return output;
    }
}
