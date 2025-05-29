using Dom.Entities;

namespace App.Interfaces;

public interface IElasticSearchService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(Guid id);
}
