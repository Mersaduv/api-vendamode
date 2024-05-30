using api_vendamode.Entities.Products;
using api_vendamode.Models;

namespace api_vendamode.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<PaginatedList<Product>> GetPaginationAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    long GetLastProductCodeNumber();
    Task<Product?> GetAsyncBy(Guid id);
    Task<Product?> GetAsyncBy(string productName);
    Task CreateAsync(Product product);
}