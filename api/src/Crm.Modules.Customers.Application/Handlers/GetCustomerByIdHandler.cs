using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Domain.Entities;

namespace Crm.Modules.Customers.Application.Handlers;

public class GetCustomerByIdHandler(ICustomerReadRepository customerReadRepository)
{
    public Task<Customer?> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
    {
        return customerReadRepository.GetByIdAsync(query.Id, cancellationToken);
    }
}