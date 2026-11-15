using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Shared.Pagination;
using JasperFx.Events.Documents;
using Microsoft.EntityFrameworkCore;

namespace Crm.Modules.Customers.Infrastructure.Persistence.Repositories;

public class CustomerReadRepository(CustomersDbContext dbContext) : ICustomerReadRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            dbContext.Customers.AsNoTracking(),
            customer => customer.Id == id,
            cancellationToken);
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name);

        var totalCount = await EntityFrameworkQueryableExtensions.CountAsync(query, cancellationToken);

        var customers = await EntityFrameworkQueryableExtensions.ToListAsync(
            query.Skip((page - 1) * pageSize).Take(pageSize),
            cancellationToken);

        return new PagedResult<Customer>(customers, page, pageSize, totalCount);
    }
}