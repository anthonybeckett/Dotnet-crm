using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Application.Validators;
using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Application.Tests.Validators;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "John Smith",
            "john.smith@example.com",
            "07700900123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "",
            "john.smith@example.com",
            "07700900123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_WithInvalidCustomerType_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            (CustomerType)99,
            "John Smith",
            "john.smith@example.com",
            "07700900123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.CustomerType));
    }

    [Fact]
    public void Validate_WithInvalidEmail_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "John Smith",
            "not-an-email",
            "07700900123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.Email));
    }


    [Fact]
    public void Validate_WithNameExceedingMaximumLength_ReturnsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            new string('A', 201),
            null,
            null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validate_WithEmailExceedingMaximumLength_ReturnsInvalid()
    {
        // Arrange
        var email = $"{new string('a', 310)}@example.com";

        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "John Smith",
            email,
            null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.Email));
    }

    [Fact]
    public void Validate_WithPhoneNumberExceedingMaximumLength_ReturnsInvalid()
    {
        // Arrange
        var phoneNumber = new string('1', 51);

        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "John Smith",
            null,
            phoneNumber);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.PhoneNumber));
    }

    [Fact]
    public void Validate_WithOptionalFieldsMissing_ReturnsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Organisation,
            "Acme Limited",
            null,
            null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}