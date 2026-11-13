using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Modules.Customers.Infrastructure.Persistence;

public class CustomersDbContext(DbContextOptions<CustomersDbContext> options)
    : DbContext(options), ICustomerRepository
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("customers");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
    }

    public void Add(Customer customer)
    {
        Customers.Add(customer);
    }
}