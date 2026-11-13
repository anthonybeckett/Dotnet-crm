using Crm.Api.Endpoints.Customers;
using Crm.Modules.Customers.Application;
using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Infrastructure;
using Crm.Modules.Customers.Infrastructure.Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomersInfrastructure(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("CrmDb")
    ?? throw new InvalidOperationException("Connection string 'CrmDb' was not configured.");

builder.Host.UseWolverine(options =>
{
    options.Discovery.IncludeAssembly(typeof(CustomersModule).Assembly);
    
    options.PersistMessagesWithPostgresql(connectionString, "wolverine");
    
    options.UseFluentValidation();

    options.UseEntityFrameworkCoreTransactions()
        .WithDbContextAbstraction<ICustomerRepository, CustomersDbContext>();

    options.Policies.AutoApplyTransactions();
});

var app = builder.Build();

// Register endpoints
app.MapCustomerEndpoints();

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