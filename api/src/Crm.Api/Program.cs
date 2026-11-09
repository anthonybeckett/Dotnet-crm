using Crm.Modules.Customers.Application;
using Crm.Modules.Customers.Infrastructure;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomersInfrastructure(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseWolverine(options =>
{
    options.Discovery.IncludeAssembly(
        typeof(CustomersModule).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    Application = "CRM API",
    Version = "0.1.0",
    Status = "Running"
}));

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy"
}));

app.Run();