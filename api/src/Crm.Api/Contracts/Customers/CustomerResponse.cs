using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Api.Contracts.Customers;

public record CustomerResponse(
    Guid Id,
    CustomerType CustomerType,
    string Name,
    string? Email,
    string? PhoneNumber,
    DateTimeOffset CreatedAtUtc,
    bool IsArchived);