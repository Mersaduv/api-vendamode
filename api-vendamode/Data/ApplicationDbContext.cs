using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using api_vendamode.Entities.Products;
using api_vendamode.Entities.Users;
using api_vendamode.Entities.Users.Security;
using api_vendamode.Entities;

namespace api_vendamode.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = default!;
    public DbSet<UserSpecification> UserSpecifications { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<EntityImage<Guid, Category>> CategoryImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Product>> ProductImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Brand>> BrandImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, ProductSize>> ProductSizeImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Review>> ReviewImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, User>> UserImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, UserSpecification>> UserSpecificationImages { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<ProductFeature> Features { get; set; } = default!;
    public DbSet<FeatureValue> FeatureValues { get; set; } = default!;
    public DbSet<ProductSize> ProductSizes { get; set; } = default!;
    public DbSet<Sizes> Sizes { get; set; } = default!;
    public DbSet<ProductSizeValues> ProductSizeValues { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    // public DbSet<Details> Details { get; set; } = default!;
}