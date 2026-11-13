using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;

namespace Crm.Modules.Customers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CrmDb")
                               ?? throw new InvalidOperationException("Connection string 'CrmDb' was not configured.");

        services.AddDbContextWithWolverineIntegration<CustomersDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ICustomerRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<CustomersDbContext>());

        return services;
    }
}