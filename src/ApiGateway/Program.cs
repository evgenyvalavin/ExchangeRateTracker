using ExchangeRateTracker.ApiGateway.Services;
using ExchangeRateTracker.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddEnvironmentVariables();

// Add HTTP client for proxy service
builder.Services.AddHttpClient<ProxyService>();

// Add services
builder.Services.AddScoped<ProxyService>();

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
