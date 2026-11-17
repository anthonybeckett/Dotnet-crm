using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Handlers;
using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Modules.Customers.Domain.Enums;
using Moq;

namespace Crm.Modules.Customers.Application.Tests.Handlers;

public class GetCustomerByIdHandlerTests
{
    private readonly Mock<ICustomerReadRepository> _repositoryMock = new();

    private readonly GetCustomerByIdHandler _handler;

    public GetCustomerByIdHandlerTests()
    {
        _handler = new GetCustomerByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        var customer = Customer.Create(
            CustomerType.Individual,
            "John Smith",
            "john.smith@example.com");

        var query = new GetCustomerByIdQuery(customer.Id);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Name, result.Name);

        _repositoryMock.Verify(repository => repository.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();

        var query = new GetCustomerByIdQuery(customerId);

        _repositoryMock.Setup(repository => repository.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);

        _repositoryMock.Verify(repository => repository.GetByIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}