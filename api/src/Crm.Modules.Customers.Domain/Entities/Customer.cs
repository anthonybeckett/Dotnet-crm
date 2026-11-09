using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Domain.Entities;

public class Customer
{
    private Customer()
    {
        Name = string.Empty;
    }

    public Guid Id { get; private set; }

    public CustomerType CustomerType { get; private set; }

    public string Name { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }
    
    public DateTimeOffset CreatedAtUtc { get; private set; }
    
    public static Customer Create(
        CustomerType customerType,
        string name,
        string? email = null,
        string? phoneNumber = null
    )
    {
        if (!Enum.IsDefined(customerType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(customerType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name is required.",
                nameof(name));
        }

        return new Customer
        {
            Id = Guid.CreateVersion7(),
            CustomerType = customerType,
            Name = name.Trim(),
            Email = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
                ? null
                : phoneNumber.Trim(),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}