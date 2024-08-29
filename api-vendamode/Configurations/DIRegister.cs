using System.Text;
using api_vendace.Framework;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IRepository;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Repository;
using api_vendace.Services.Auth;
using api_vendace.Services.Products;
using api_vendace.Utility;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Services;
using api_vendamode.Services.Auth;
using api_vendamode.Services.Design;
using api_vendamode.Services.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace api_vendace.Configurations;

public static class DIRegister
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    public static void AddApplicationServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddScoped(_ => appSettings)
                .AddScoped<IProductServices, ProductServices>()
                .AddScoped<IBrandServices, BrandServices>()
                .AddScoped<IUserServices, UserServices>()
                .AddScoped<ICategoryServices, CategoryServices>()
                .AddScoped<IReviewServices, ReviewServices>()
                .AddScoped<IFeatureServices, FeatureServices>()
                .AddScoped<IProductSizeServices, ProductSizeServices>()
                .AddScoped<IPermissionServices, PermissionServices>()
                .AddScoped<IRoleServices, RoleServices>()
                .AddScoped<IAddressServices, AddressServices>()
                .AddScoped<ISliderServices, SliderServices>()
                .AddScoped<IOrderServices, OrderService>()
                .AddScoped<ICanceledServices, CanceledServices>()
                .AddScoped<IDesignServices, DesignServices>()
                .AddScoped<IBannerServices, BannerServices>()
                .AddScoped<IArticleServices, ArticleServices>();
        // services.AddScoped<IPaymentService, PaymentService>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddInfraUtility(this IServiceCollection services)
    {
        services.AddScoped<ByteFileUtility>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    public static IServiceCollection AddJWT(this IServiceCollection services, AppSettings appSettings)
    {
        var sp = services.BuildServiceProvider();
        var key = Encoding.UTF8.GetBytes(appSettings.AuthSettings.TokenKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromDays(appSettings.AuthSettings.TokenTimeout),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Api Venda Mode",
                Description = "Api Venda Mode - Version 01",
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
            });

            c.EnableAnnotations();
        });
        return services;
    }
}