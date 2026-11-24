using System.Threading;
using System.Threading.Tasks;
using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Shared.Pagination;

namespace Crm.Modules.Customers.Application.Handlers;

public class GetCustomersHandler(ICustomerReadRepository customerReadRepository)
{
    public async Task<PagedResult<Customer>> Handle(GetCustomersQuery query, CancellationToken cancellationToken)
    {
        return await customerReadRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.IncludeArchived,
            cancellationToken);
    }
}