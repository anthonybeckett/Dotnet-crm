using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Application.Validators;
using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Application.Tests.Validators;

public class UpdateCustomerCommandValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            CustomerType.Individual,
            "John Smith",
            "john.smith@example.com",
            "0123456789");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_ReturnsInvalid()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            CustomerType.Individual,
            "",
            null,
            null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithInvalidCustomerType_ReturnsInvalid()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            (CustomerType)999,
            "John Smith",
            null,
            null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "CustomerType");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ReturnsInvalid()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            CustomerType.Individual,
            "John Smith",
            "not-an-email",
            null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Email");
    }
}