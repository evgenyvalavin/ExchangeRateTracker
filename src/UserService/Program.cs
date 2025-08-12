using System.Text;
using ExchangeRateTracker.Common.Data;
using ExchangeRateTracker.Common.Interfaces;
using ExchangeRateTracker.Common.Services;
using ExchangeRateTracker.UserService.Grpc;
using ExchangeRateTracker.UserService.Interfaces;
using ExchangeRateTracker.UserService.Repositories;
using ExchangeRateTracker.UserService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddEnvironmentVariables();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=exchangeratetrackerdb;Username=postgres;Password=postgres123";
    options.UseNpgsql(connectionString);
});

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Add JWT authentication
var jwtSecretKey = builder.Configuration["JWT:SecretKey"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure";
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "ExchangeRateTracker";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "ExchangeRateTracker";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

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
app.MapGrpcService<AuthGrpcService>();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
