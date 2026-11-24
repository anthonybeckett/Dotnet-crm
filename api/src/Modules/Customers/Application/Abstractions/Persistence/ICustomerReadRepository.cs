using System;
using System.Threading;
using System.Threading.Tasks;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Shared.Pagination;

namespace Crm.Modules.Customers.Application.Abstractions.Persistence;

public interface ICustomerReadRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
}