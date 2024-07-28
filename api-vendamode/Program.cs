using api_vendace.Configurations;
using api_vendace.Const;
using api_vendace.Data;
using api_vendace.Endpoints;
using api_vendace.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using api_vendamode.Endpoints;
using Npgsql;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    Args = args
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var config = builder.Configuration;
var appSettings = config.Get<AppSettings>() ?? new AppSettings();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(appSettings);
builder.Services.AddRepositories();
builder.Services.AddUnitOfWork();
//register DbContext
string connectionString = builder.Configuration.GetConnectionString("SqlConnection") ?? string.Empty;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    // dataSourceBuilder.EnableDynamicJson();
    // options.UseNpgsql(dataSourceBuilder.Build());
    options.UseNpgsql(connectionString);
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddJWT(appSettings);
builder.Services.AddSwagger();
builder.Services.AddOptions();
builder.Services.AddCors();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddInfraUtility();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Venda Mode");
    });
// }
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

var apiGroup = app.MapGroup(Constants.Api);
apiGroup
    .MapProductApi()
    .MapProductFeatureApi()
    .MapBrandApi()
    .MapCategoryApi()
    .MapAuthApi()
    .MapProductSizeApi()
    .MapAddressApi()
    .MapSliderApi()
    .MapReviewApi()
    .MapOrderApi()
    .MapCanceledApi();

// app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

app.Run();