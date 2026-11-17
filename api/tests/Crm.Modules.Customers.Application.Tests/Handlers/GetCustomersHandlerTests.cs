using Crm.Modules.Customers.Application.Abstractions.Persistence;
using Crm.Modules.Customers.Application.Handlers;
using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Modules.Customers.Domain.Enums;
using Crm.Shared.Pagination;
using Moq;

namespace Crm.Modules.Customers.Application.Tests.Handlers;

public class GetCustomersHandlerTests
{
    private readonly Mock<ICustomerReadRepository> _repositoryMock = new();

    private readonly GetCustomersHandler _handler;

    public GetCustomersHandlerTests()
    {
        _handler = new GetCustomersHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsPagedCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            Customer.Create(CustomerType.Individual, "John Smith"),
            Customer.Create(CustomerType.Organisation, "Acme Limited")
        };

        const int page = 1;
        const int pageSize = 10;
        const int totalCount = 2;

        var pagedResult = new PagedResult<Customer>(
            customers,
            page,
            pageSize,
            totalCount);

        var query = new GetCustomersQuery(page, pageSize);

        _repositoryMock.Setup(repository => repository.GetPagedAsync(page, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(totalCount, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("John Smith", result.Items[0].Name);
        Assert.Equal("Acme Limited", result.Items[1].Name);

        _repositoryMock.Verify(repository => repository.GetPagedAsync(page, pageSize, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}