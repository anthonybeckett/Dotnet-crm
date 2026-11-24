using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Crm.Modules.Customers.Infrastructure.Persistence.Repositories;

public class CustomerReadRepository(CustomersDbContext dbContext) : ICustomerReadRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(customer => customer.Id == id,
            cancellationToken);
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<Customer>(customers, page, pageSize, totalCount);
    }
}