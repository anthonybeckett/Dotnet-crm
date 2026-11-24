using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Commands;

namespace Crm.Modules.Customers.Application.Handlers;

public class UpdateCustomerHandler(ICustomerRepository customerRepository)
{
    public async Task Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(command.Id, cancellationToken);

        if (customer is null)
        {
            throw new KeyNotFoundException($"Customer with ID '{command.Id}' was not found.");
        }

        customer.Update(command.CustomerType, command.Name, command.Email, command.PhoneNumber);
    }
}