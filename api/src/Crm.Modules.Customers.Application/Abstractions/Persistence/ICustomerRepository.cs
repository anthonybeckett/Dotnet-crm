using Crm.Modules.Customers.Domain.Entities;

namespace Crm.Modules.Customers.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    void Add(Customer customer);
}