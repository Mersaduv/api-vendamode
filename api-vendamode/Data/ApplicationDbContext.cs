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
using api_vendamode.Entities;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendamode.Utility;
using api_vendamode.Models.Dtos;
using api_vendamode.Entities.Designs;
using System.Text.Json;

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

        modelBuilder.Entity<ProductSizeProductSizeValue>()
        .HasKey(pspsv => new { pspsv.ProductSizeId, pspsv.ProductSizeValueId });

        modelBuilder.Entity<ProductSizeProductSizeValue>()
            .HasOne(pspsv => pspsv.ProductSize)
            .WithMany(ps => ps.ProductSizeProductSizeValues)
            .HasForeignKey(pspsv => pspsv.ProductSizeId);

        modelBuilder.Entity<ProductSizeProductSizeValue>()
            .HasOne(pspsv => pspsv.ProductSizeValue)
            .WithMany(psv => psv.ProductSizeProductSizeValues)
            .HasForeignKey(pspsv => pspsv.ProductSizeValueId);

        modelBuilder.Entity<ProductSizeValues>()
            .HasIndex(psv => psv.Name)
            .IsUnique();

        modelBuilder.Entity<CategorySize>()
        .HasKey(cs => cs.Id);

        modelBuilder.Entity<CategorySize>()
            .HasOne(cs => cs.Category)
            .WithMany(c => c.CategorySizes)
            .HasForeignKey(cs => cs.CategoryId);

        modelBuilder.Entity<CategorySize>()
            .HasOne(cs => cs.Size)
            .WithMany(s => s.CategorySizes)
            .HasForeignKey(cs => cs.SizeId);

        //...
        modelBuilder.Entity<CategoryProductSize>()
        .HasKey(cs => new { cs.CategoryId, cs.ProductSizeId });

        modelBuilder.Entity<CategoryProductSize>()
            .HasOne(cs => cs.Category)
            .WithMany(c => c.CategoryProductSizes)
            .HasForeignKey(cs => cs.CategoryId);

        modelBuilder.Entity<CategoryProductSize>()
            .HasOne(cs => cs.ProductSize)
            .WithMany(s => s.CategoryProductSizes)
            .HasForeignKey(cs => cs.ProductSizeId);

        modelBuilder.Entity<CategoryProductFeature>()
            .HasKey(cpf => new { cpf.CategoryId, cpf.ProductFeatureId });

        modelBuilder.Entity<CategoryProductFeature>()
            .HasOne(cpf => cpf.Category)
            .WithMany(c => c.CategoryProductFeatures)
            .HasForeignKey(cpf => cpf.CategoryId);

        modelBuilder.Entity<CategoryProductFeature>()
            .HasOne(cpf => cpf.ProductFeature)
            .WithMany(pf => pf.CategoryProductFeatures)
            .HasForeignKey(cpf => cpf.ProductFeatureId);

        var converter = new DictionaryToJsonConverter();

        modelBuilder.Entity<StockItem>()
            .Property(e => e.AdditionalProperties)
            .HasConversion(converter!)
            .HasColumnType("jsonb");
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<Canceled> Canceleds { get; set; }
    public DbSet<Returned> Returneds { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<UserSpecification> UserSpecifications { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<DescriptionEntity> Descriptions { get; set; }
    public DbSet<LogoImages> LogoImages { get; set; }
    public DbSet<EntityMainImage<Guid, LogoImages>> OrgThumbnailImages { get; set; }
    public DbSet<EntityImage<Guid, LogoImages>> FaviconThumbnailImages { get; set; }
    public DbSet<EntityImage<Guid, Category>> CategoryImages { get; set; }
    public DbSet<EntityImage<Guid, Product>> ProductImages { get; set; }
    public DbSet<EntityMainImage<Guid, Product>> ProductMainImages { get; set; }
    public DbSet<EntityImage<Guid, Brand>> BrandImages { get; set; }
    public DbSet<EntityImage<Guid, ProductSize>> ProductSizeImages { get; set; }
    public DbSet<EntityImage<Guid, Review>> ReviewImages { get; set; }
    public DbSet<EntityImage<Guid, User>> UserImages { get; set; }
    public DbSet<EntityImage<Guid, UserSpecification>> UserSpecificationImages { get; set; }
    public DbSet<EntityImage<Guid, Slider>> SliderImages { get; set; }
    public DbSet<EntityImage<Guid, Order>> PurchaseInvoice { get; set; }
    public DbSet<EntityImage<Guid, StockItem>> StockImages { get; set; }
    public DbSet<EntityImage<Guid, DescriptionEntity>> MediaImages { get; set; }
    public DbSet<EntityImage<Guid, Banner>> BannerImages { get; set; }
    public DbSet<EntityImage<Guid, Article>> ArticleImages { get; set; }
    public DbSet<EntityImage<Guid, FooterBanner>> FooterBannerImages { get; set; }
    public DbSet<EntityImage<Guid, DesignItem>> DesignItemImages { get; set; }
    public DbSet<StoreCategory> StoreCategories { get; set; }
    public DbSet<GeneralSetting> GeneralSettings { get; set; }
    public DbSet<DesignItem> DesignItems { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleReview> ArticleReviews { get; set; }
    public DbSet<ArticleBanner> ArticleBanners { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<FooterBanner> FooterBanners { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductFeature> ProductFeatures { get; set; }
    public DbSet<CategoryProductFeature> CategoryProductFeatures { get; set; }
    public DbSet<FeatureValue> FeatureValues { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<ProductSizeProductSizeValue> ProductSizeProductSizeValues { get; set; }
    public DbSet<CategoryProductSize> CategoryProductSizes { get; set; }
    public DbSet<Sizes> Sizes { get; set; }
    public DbSet<CategorySize> CategorySizes { get; set; }
    public DbSet<ProductScale> ProductScales { get; set; }
    public DbSet<SizeIds> SizeIds { get; set; }
    public DbSet<SizeModel> SizeModels { get; set; }
    public DbSet<ProductSizeValues> ProductSizeValues { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<HeaderText> HeaderTexts { get; set; }
    public DbSet<SloganFooter> SloganFooters { get; set; }
    public DbSet<Support> Supports { get; set; }
    public DbSet<Redirects> Redirects { get; set; }
    public DbSet<Copyright> Copyrights { get; set; }
    public DbSet<ColumnFooter> ColumnFooters { get; set; }
    public DbSet<FooterArticleColumn> FooterArticleColumns { get; set; }
}