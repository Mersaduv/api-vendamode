using System.Text;
using api_vendamode.Framework;
using api_vendamode.Interfaces;
using api_vendamode.Interfaces.IRepository;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Repository;
using api_vendamode.Services.Auth;
using api_vendamode.Services.Products;
using api_vendamode.Utility;
using ApiAryanakala.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace api_vendamode.Configurations;

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
                .AddScoped<IUserServices, UserServices>()
                .AddScoped<ICategoryServices, CategoryServices>()
                .AddScoped<IReviewServices, ReviewServices>()
                .AddScoped<IPermissionServices, PermissionServices>()
                .AddScoped<IFeatureServices, FeatureServices>()
                .AddScoped<IProductSizeServices, ProductSizeServices>();
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
                ClockSkew = TimeSpan.FromMinutes(appSettings.AuthSettings.TokenTimeout),
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