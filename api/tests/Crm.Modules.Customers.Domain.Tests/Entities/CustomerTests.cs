using Crm.Modules.Customers.Domain.Entities;
using Crm.Modules.Customers.Domain.Enums;

namespace Crm.Modules.Customers.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidIndividualDetails_ReturnsCustomer()
    {
        // Arrange
        const string name = "John Smith";
        const string email = "john.smith@example.com";
        const string phoneNumber = "07700900123";

        // Act
        var customer = Customer.Create(
            CustomerType.Individual,
            name,
            email,
            phoneNumber);

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(CustomerType.Individual, customer.CustomerType);
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
        Assert.Equal(phoneNumber, customer.PhoneNumber);
    }


    [Fact]
    public void Create_WithValidOrganisationDetails_ReturnsOrganisation()
    {
        // Arrange
        const string name = "Acme Limited";

        // Act
        var customer = Customer.Create(CustomerType.Organisation, name);

        // Assert
        Assert.Equal(CustomerType.Organisation, customer.CustomerType);
        Assert.Equal(name, customer.Name);
        Assert.Null(customer.Email);
        Assert.Null(customer.PhoneNumber);
    }

    [Fact]
    public void Create_WithWhitespaceAroundDetails_TrimsValues()
    {
        // Arrange
        const string name = "  John Smith  ";
        const string email = "  john.smith@example.com  ";
        const string phoneNumber = "  07700900123  ";

        // Act
        var customer = Customer.Create(
            CustomerType.Individual,
            name,
            email,
            phoneNumber);

        // Assert
        Assert.Equal("John Smith", customer.Name);
        Assert.Equal("john.smith@example.com", customer.Email);
        Assert.Equal("07700900123", customer.PhoneNumber);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        const string name = "";

        // Act
        var exception = Assert.Throws<ArgumentException>(() => Customer.Create(CustomerType.Individual, name));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCustomerType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidCustomerType = (CustomerType)99;

        // Act
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => Customer.Create(invalidCustomerType, "John Smith"));

        // Assert
        Assert.Equal("customerType", exception.ParamName);
    }


    [Fact]
    public void Create_GeneratesVersion7Guid()
    {
        // Act
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(7, customer.Id.Version);
    }

    [Fact]
    public void Create_SetsCreationTimestampToUtc()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        Assert.InRange(customer.CreatedAtUtc, beforeCreation, afterCreation);

        Assert.Equal(TimeSpan.Zero, customer.CreatedAtUtc.Offset);
    }

    [Fact]
    public void Update_WithValidDetails_UpdatesCustomer()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Act
        customer.Update(CustomerType.Organisation,
            "  Example Ltd  ",
            " contact@example.com ",
            " 0123456789 ");

        // Assert
        Assert.Equal(CustomerType.Organisation, customer.CustomerType);
        Assert.Equal("Example Ltd", customer.Name);
        Assert.Equal("contact@example.com", customer.Email);
        Assert.Equal("0123456789", customer.PhoneNumber);
    }

    [Fact]
    public void Update_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => customer.Update(CustomerType.Individual, ""));
    }

    [Fact]
    public void Update_WithInvalidCustomerType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => customer.Update((CustomerType)99, "John Smith"));
    }
    
    [Fact]
    public void Archive_SetsIsArchivedToTrue()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Act
        customer.Archive();

        // Assert
        Assert.True(customer.IsArchived);
    }
    
    [Fact]
    public void Create_WithValidDetails_SetsIsArchivedToFalse()
    {
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        Assert.False(customer.IsArchived);
    }
    
    [Fact]
    public void Restore_WithArchivedCustomer_SetsIsArchivedToFalse()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        customer.Archive();

        // Act
        customer.Restore();

        // Assert
        Assert.False(customer.IsArchived);
    }

    [Fact]
    public void Restore_WithActiveCustomer_LeavesIsArchivedFalse()
    {
        // Arrange
        var customer = Customer.Create(CustomerType.Individual, "John Smith");

        // Act
        customer.Restore();

        // Assert
        Assert.False(customer.IsArchived);
    }
}