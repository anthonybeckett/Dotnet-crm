using Crm.Modules.Customers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Modules.Customers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CrmDb")
                               ?? throw new InvalidOperationException("Connection string 'CrmDb' was not configured.");

        services.AddDbContext<CustomersDbContext>(options => { options.UseNpgsql(connectionString); });

        return services;
    }
}