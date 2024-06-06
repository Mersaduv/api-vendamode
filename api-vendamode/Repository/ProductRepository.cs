using api_vendace.Data;
using api_vendace.Entities.Products;
using api_vendace.Interfaces.IRepository;
using api_vendace.Interfaces.IServices;
using api_vendace.Mapper;
using api_vendace.Models;
using api_vendace.Utility;
using Microsoft.EntityFrameworkCore;
namespace api_vendace.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateAsync(Product product)
    {
        await _context.AddAsync(product);
    }

    public async Task<PaginatedList<Product>> GetPaginationAsync(int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(x => x.Brand)
            .Include(x => x.Images)
            .Include(x => x.ProductFeatures)
            .Include(x => x.ProductScale)
            .Include(x => x.Review)
            .Include(c => c.Category)
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Product>
        {
            Data = products,
            TotalCount = totalCount,
            CurrentPage = pageNumber,
            PageSize = pageSize,
        };
    }
    public IQueryable<Product> GetQuery()
    {
        var query = _context.Products
                            .Include(x => x.Brand)
                            .Include(x => x.Images)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .AsQueryable();
        return query;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var productAll = _context.Products
        .Include(x => x.Brand)
        .Include(x => x.Images)
        .Include(x => x.ProductFeatures)
        .Include(x => x.ProductScale)
        .Include(x => x.Review)
        .Include(c => c.Category)
        .AsNoTracking()
        .ToListAsync();

        return await productAll;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<Product?> GetAsyncBy(Guid id)
    {
        return await _context.Products
        .Include(x => x.Brand)
        .Include(x => x.Images)
        .Include(x => x.ProductFeatures)
        .Include(x => x.ProductScale)
        .Include(x => x.Review)
        .Include(c => c.Category)
        .AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Product?> GetAsyncBy(string productName)
    {
        return await _context.Products
        .Include(x => x.Brand)
        .Include(x => x.Images)
        .Include(x => x.ProductFeatures)
        .Include(x => x.ProductScale)
        .Include(x => x.Review)
        .Include(c => c.Category)
        .FirstOrDefaultAsync(u => u.Title.ToLower() == productName.ToLower());
    }

    public long GetLastProductCodeNumber()
    {
        // Get the last product from the database, ordered by the product code in descending order
        var lastProduct = _context.Products
            .OrderByDescending(p => p.Code)
            .FirstOrDefault();

        // If there are no products in the database, return 0
        if (lastProduct == null)
        {
            return 0;
        }

        // Extract the numeric part from the last product code, convert it to a long, and return it
        string numericPart = lastProduct.Code.Substring(1); // Remove the 'K' prefix
        long lastCodeNumber = long.Parse(numericPart);
        return lastCodeNumber;
    }
}