using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using api_vendace.Entities.Products;
using api_vendace.Entities.Users;
using api_vendace.Entities.Users.Security;
using api_vendace.Entities;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendamode.Entities.Products;

namespace api_vendace.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Review>()
            .HasMany(r => r.PositivePoints)
            .WithOne()
            .HasForeignKey("PositiveReviewId");

        modelBuilder.Entity<Review>()
            .HasMany(r => r.NegativePoints)
            .WithOne()
            .HasForeignKey("NegativeReviewId");

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductScale)
            .WithOne(ps => ps.Product)
            .HasForeignKey<Product>(p => p.ProductScaleId);

        modelBuilder.Entity<Address>().OwnsOne(a => a.Province);
        modelBuilder.Entity<Address>().OwnsOne(a => a.City);
    }
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Address> Addresses { get; set; } = default!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = default!;
    public DbSet<UserSpecification> UserSpecifications { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Slider> Sliders { get; set; } = default!;
    public DbSet<EntityImage<Guid, Category>> CategoryImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Product>> ProductImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Brand>> BrandImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, ProductSize>> ProductSizeImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Review>> ReviewImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, User>> UserImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, UserSpecification>> UserSpecificationImages { get; set; } = default!;
    public DbSet<EntityImage<Guid, Slider>> SliderImages { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;
    public DbSet<ProductFeature> ProductFeatures { get; set; } = default!;
    public DbSet<FeatureValue> FeatureValues { get; set; } = default!;
    public DbSet<ProductSize> ProductSizes { get; set; } = default!;
    public DbSet<Sizes> Sizes { get; set; } = default!;
    public DbSet<ProductScale> ProductScales { get; set; } = default!;
    public DbSet<SizeIds> SizeIds { get; set; } = default!;
    public DbSet<SizeModel> SizeModels { get; set; } = default!;
    public DbSet<ProductSizeValues> ProductSizeValues { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    // public DbSet<Details> Details { get; set; } = default!;
}