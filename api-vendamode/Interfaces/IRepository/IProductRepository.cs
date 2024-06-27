using api_vendace.Entities.Products;
using api_vendace.Models;

namespace api_vendace.Interfaces.IRepository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<PaginatedList<Product>> GetPaginationAsync(int pageNumber, int pageSize);
    IQueryable<Product> GetQuery();
    Task<int> GetTotalCountAsync();
    long GetLastProductCodeNumber();
    Task<Product?> GetAsyncBy(Guid id);
    Task<Product?> GetAsyncBy(string slug);
    Task CreateAsync(Product product);
}