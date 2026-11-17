using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Application.Handlers;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Modules.Customers.Domain.Enums;
using Moq;

namespace Crm.Modules.Customers.Application.Tests.Handlers;

public class CreateCustomerHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();

    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _handler = new CreateCustomerHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsId()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            CustomerType.Individual,
            "John Smith",
            "john.smith@example.com",
            "07700900123");

        // Act
        var customerId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, customerId);

        _repositoryMock.Verify(
            repository => repository.Add(
                It.Is<Customer>(customer =>
                    customer.Id == customerId &&
                    customer.CustomerType == CustomerType.Individual &&
                    customer.Name == "John Smith" &&
                    customer.Email == "john.smith@example.com" &&
                    customer.PhoneNumber == "07700900123")),
            Times.Once);
    }
}