using System;
using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Application.Commands;

public record UpdateCustomerCommand(
    Guid Id,
    CustomerType CustomerType,
    string Name,
    string? Email,
    string? PhoneNumber
);