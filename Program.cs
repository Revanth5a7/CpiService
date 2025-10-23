using CpiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add memory cache (needed later for caching service)
builder.Services.AddMemoryCache();

// Add HTTP client for BLS API service
builder.Services.AddHttpClient<BlsApiService>();

// Add CPI cache service
builder.Services.AddScoped<CpiCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// No authentication yet, just controllers
app.UseAuthorization();

app.MapControllers();

app.Run();
