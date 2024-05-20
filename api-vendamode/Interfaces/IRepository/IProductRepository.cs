using api_vendamode.Entities.Products;

namespace api_vendamode.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<int> GetTotalCountAsync();
    Task<Product?> GetAsyncBy(Guid id);
    Task<Product?> GetAsyncBy(string productName);
    Task CreateAsync(Product product);
}