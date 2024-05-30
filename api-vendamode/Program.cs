using api_vendamode.Configurations;
using api_vendamode.Const;
using api_vendamode.Data;
using api_vendamode.Endpoints;
using api_vendamode.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(connectionString);
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddJWT(appSettings);
builder.Services.AddOptions();
builder.Services.AddCors();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddInfraUtility();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod());
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

var apiGroup = app.MapGroup(Constants.Api);
apiGroup
    .MapProductApi()
    .MapProductFeatureApi()
    .MapProductFeatureApi();

app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

app.Run();