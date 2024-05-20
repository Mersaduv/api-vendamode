using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using api_vendamode.Entities.Products;

namespace api_vendamode.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<EntityImage<int, Category>> CategoryImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Product>> ProductImages { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<ProductInfo> ProductInformation { get; set; } = default!;
    public DbSet<ProductSpecification> ProductSpecification { get; set; } = default!;
    public DbSet<ProductColor> ProductColors { get; set; } = default!;
    // public DbSet<Details> Details { get; set; } = default!;
}