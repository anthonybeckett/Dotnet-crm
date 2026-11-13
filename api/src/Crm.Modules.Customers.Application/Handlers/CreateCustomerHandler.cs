using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Domain.Entities;

namespace Crm.Modules.Customers.Application.Handlers;

public class CreateCustomerHandler
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public Task<Guid> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var customer = Customer.Create(
                command.CustomerType,
                command.Name,
                command.Email,
                command.PhoneNumber);

            _customerRepository.Add(customer);

            return Task.FromResult(customer.Id);
        }
        catch (Exception exception)
        {
            return Task.FromException<Guid>(exception);
        }
    }
}