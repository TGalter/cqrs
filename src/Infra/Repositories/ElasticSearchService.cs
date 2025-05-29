
using App.Interfaces;
using Dom.Entities;
using Microsoft.Extensions.Options;
using Nest;

namespace Infra.Repositories;

public class ElasticSearchService : IElasticSearchService
{
    private readonly ElasticClient _client;
    private readonly string _index;

    public ElasticSearchService(IOptions<ElasticSearchSettings> settings)
    {
        var config = settings.Value;
        _index = config.Index;

        var connectionSettings = new ConnectionSettings(new Uri(config.Uri))
            .DefaultIndex(_index);

        _client = new ElasticClient(connectionSettings);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var response = await _client.SearchAsync<Product>(s => s
            .Index(_index)
            .Query(q => q.MatchAll())
        );

        return response.Documents;
    }

    public async Task<Product> GetProductByIdAsync(Guid id)
    {
        var response = await _client.GetAsync<Product>(id, g => g.Index(_index));

        if (response.Found)
        {
            return response.Source;
        }

        throw new KeyNotFoundException($"Product with ID {id} not found.");
    }
}
