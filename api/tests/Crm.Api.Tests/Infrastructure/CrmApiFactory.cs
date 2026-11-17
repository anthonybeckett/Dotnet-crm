using Crm.Modules.Customers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Crm.Api.Tests.Infrastructure;

public class CrmApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConfiguration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var testConnectionString = testConfiguration.GetConnectionString("CrmTestDb") ?? throw new InvalidOperationException("CrmTestDb connection string was not found in User Secrets.");

        builder.UseSetting("ConnectionStrings:CrmDb", testConnectionString);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();

        Console.WriteLine($"Test database: {dbContext.Database.GetDbConnection().Database}");

        dbContext.Database.Migrate();

        return host;
    }
}