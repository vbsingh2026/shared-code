using SmartCart.Application.Coupons;
using SmartCart.Application.Coupons.Strategies;
using SmartCart.Application.Interfaces;
using SmartCart.Application.Services;
using SmartCart.Infrastructure.Repositories;
using SmartCart.Api.Middlewares;
using SmartCart.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register custom logger service
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();


// Configure CORS for local dev UI (adjust origins as needed)
const string AllowLocalDevCorsPolicy = "AllowLocalDevCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowLocalDevCorsPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Vite default
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISmartCartService, SmartCartService>();
builder.Services.AddSingleton<ICartRepository, InMemoryCartRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<IOrderRepository, InMemoryOrderRepository>();

builder.Services.AddScoped<ICouponFactory, CouponFactory>();
builder.Services.AddScoped<ICouponStrategy, FlatCouponStrategy>();
builder.Services.AddScoped<ICouponStrategy, PercentageCouponStrategy>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCartExceptionHandling();
app.UseHttpsRedirection();

// Apply the named CORS policy
app.UseCors(AllowLocalDevCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
