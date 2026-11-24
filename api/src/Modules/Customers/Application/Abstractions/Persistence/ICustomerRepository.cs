using System;
using System.Threading;
using System.Threading.Tasks;
using Crm.Modules.Customers.Domain.Entities;

namespace Crm.Modules.Customers.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    void Add(Customer customer);
    
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}