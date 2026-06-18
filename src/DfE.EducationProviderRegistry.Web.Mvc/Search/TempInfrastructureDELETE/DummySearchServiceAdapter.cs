using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.TempInfrastructureDELETE;

public class DummySearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    public Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest searchServiceAdapterRequest,
        CancellationToken cancellationToken = default)
    {
        List<EstablishmentSearchResult> establishments = [];

        for (int i = 1; i <= 200; i++)
        {
            establishments.Add(
                new EstablishmentSearchResult(
                    urn: 100000 + i,
                    name: $"Mock Establishment {i}"
                ));
        }

        EstablishmentSearchResults establishmentResults =
            new(establishments);

        List<SearchFacet> facetList =
        [
            new SearchFacet(
                "Region",
                [
                    new("North", 80),
                    new("South", 60),
                    new("Midlands", 60)
                ]),

            new SearchFacet(
                "ProviderType",
                [
                    new("Academy", 120),
                    new("LA Maintained", 50),
                    new("Independent", 30)
                ])
        ];

        SearchFacets facets = new(facetList);

        SearchResults<EstablishmentSearchResults, SearchFacets> response =
            new()
            {
                Results = establishmentResults,
                FacetResults = facets
            };

        return Task.FromResult(response);
    }
}
