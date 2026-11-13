using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Application.Commands;

public record CreateCustomerCommand(
    CustomerType CustomerType,
    string Name,
    string? Email,
    string? PhoneNumber
);