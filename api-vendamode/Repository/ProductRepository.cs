using api_vendamode.Data;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IRepository;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Mapper;
using api_vendamode.Utility;
using Microsoft.EntityFrameworkCore;
namespace api_vendamode.Repository;

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

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var productAll = _context.Products.Include(x => x.Info)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .Include(c=>c.Category)
        .ThenInclude(x=>x.ParentCategory)
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
        return await _context.Products.Include(x => x.Info)
        .Include(x=> x.Category)
        .ThenInclude(x=>x.ParentCategory)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Product?> GetAsyncBy(string productName)
    {
        return await _context.Products.Include(x => x.Info)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .FirstOrDefaultAsync(u => u.Title.ToLower() == productName.ToLower());
    }

}