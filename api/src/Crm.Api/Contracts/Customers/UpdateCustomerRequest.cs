using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Api.Contracts.Customers;

public record UpdateCustomerRequest(CustomerType CustomerType, string Name, string? Email, string? PhoneNumber);