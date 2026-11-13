using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Api.Contracts.Customers;

public record CreateCustomerRequest(CustomerType CustomerType, string Name, string? Email, string? PhoneNumber);