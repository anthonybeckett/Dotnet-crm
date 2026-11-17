using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Application.Validators;

namespace Crm.Modules.Customers.Application.Tests.Validators;

public class GetCustomersQueryValidatorTests
{
    private readonly GetCustomersQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidPagination_ReturnsValid()
    {
        // Arrange
        var query = new GetCustomersQuery(1, 20);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithPageZero_ReturnsInvalid()
    {
        // Arrange
        var query = new GetCustomersQuery(0, 20);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(query.Page));
    }

    [Fact]
    public void Validate_WithPageSizeZero_ReturnsInvalid()
    {
        // Arrange
        var query = new GetCustomersQuery(1, 0);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(query.PageSize));
    }

    [Fact]
    public void Validate_WithPageSizeAboveMaximum_ReturnsInvalid()
    {
        // Arrange
        var query = new GetCustomersQuery(1, 101);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(query.PageSize));
    }
}