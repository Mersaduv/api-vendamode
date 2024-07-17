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
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .Include(c => c.StockItems);

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
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .ThenInclude(c=>c.ParentCategory)
                            .Include(c => c.StockItems)
                            .OrderByDescending(product => product.LastUpdated)
                            .AsQueryable();
        return query;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var productAll = _context.Products
                           .Include(x => x.Brand)
                            .Include(x => x.Images)
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .Include(c => c.StockItems)
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
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .Include(c => c.StockItems)
                            .ThenInclude(x => x.Images).FirstOrDefaultAsync(u => u.Id == id);
    }


    public async Task<Product?> GetAsyncBy(string slug)
    {
        return await _context.Products
                            .Include(x => x.Brand)
                            .Include(x => x.Images)
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .Include(c => c.StockItems)
                            .ThenInclude(x => x.Images)
        .FirstOrDefaultAsync(u => u.Slug == slug);
    }

    public long GetLastProductCodeNumber()
    {
        var lastProduct = _context.Products
           .OrderByDescending(p => p.Code)
           .FirstOrDefault();

        if (lastProduct == null)
        {
            return 0;
        }

        string prodNum = lastProduct.Code;
        string numericPart = new string(prodNum.Where(char.IsDigit).ToArray());

        if (long.TryParse(numericPart, out long lastCodeNumber))
        {
            return lastCodeNumber;
        }
        else
        {
            throw new FormatException($"Invalid order number format: {prodNum}");
        }
    }
}