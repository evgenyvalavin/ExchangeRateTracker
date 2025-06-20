using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Extensions;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.FinanceService.Grpc;
using TrueCodeTestTask.FinanceService.Repositories;
using TrueCodeTestTask.FinanceService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddEnvironmentVariables();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=truecodedb;Username=postgres;Password=postgres123";
    options.UseNpgsql(connectionString);
});

// Add repositories
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Database should be initialized by DatabaseMigration service

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map gRPC services
app.MapGrpcService<CurrencyGrpcService>();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
