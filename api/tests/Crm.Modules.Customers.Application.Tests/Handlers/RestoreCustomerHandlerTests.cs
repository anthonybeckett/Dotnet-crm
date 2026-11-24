using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Application.Handlers;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Modules.Customers.Domain.Enums;
using Moq;

namespace Crm.Modules.Customers.Application.Tests.Handlers;

public class RestoreCustomerHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly RestoreCustomerHandler _handler;

    public RestoreCustomerHandlerTests()
    {
        _handler = new RestoreCustomerHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithArchivedCustomer_RestoresCustomer()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        customer.Archive();

        var command = new RestoreCustomerCommand(customer.Id);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(customer.IsArchived);

        _repositoryMock.Verify(repository => repository.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithActiveCustomer_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        var command = new RestoreCustomerCommand(customer.Id);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.False(customer.IsArchived);
    }

    [Fact]
    public async Task Handle_WithUnknownCustomer_ThrowsKeyNotFoundException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();

        var command = new RestoreCustomerCommand(customerId);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}